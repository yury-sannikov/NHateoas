# NHateoas

> **Tip:**   Currently NHateoas in development stage.

HATEOAS (Hypermedia as the engine of application state) implementation for ASP.Net WebAPI

You can simply embed any hypermedia information into your Web.API controller's response.

Consider following example

Model
```C#
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```
Controller
```C#
public class ValuesController : ApiController
{
    private static readonly Product[] Products = { 
            new Product() { Id = 1, Name = "Item1", Price = 2.99m},
            new Product() { Id = 2, Name = "Item2", Price = 3.99m}
        }; 
    public IEnumerable<Product> Get()
    {
        return Products;
    }
    public Product Get(int id)
    {
        return Products.First();
    }
}
```

Using GET request you will get following response

```json
[
    {
        "Id":1,
        "Name":"Item1",
        "Price":2.99
   },
    {
        "Id":2,
        "Name":"Item2",
        "Price":3.99
    }
]
```

In order to perform any operation with this API UI should know and hard code API routing information.

## What issues do we have with that?
 - Tight coupling between API and UI code
 - Changing route information probably break UI functionality
 - Every minor API change requires full UI regression test

## What problem does it address?

HNateoas allows you to specify mapping between controller actions and model. HNateoas will generate hypermedia information and put it into your response object. Whenever you change routing for your API, NHateoas will reflect those changes and update hypermedia information.

## Who cares?

 - Any meduim and large teams who separate API and UI development

## How does it work?

HNateoas uses IActionFilter to create proxy object containing all your model information along with hypermedia information. On API level you don't need to add any links information into your result object. You just returning POCO model object and HNateoas does all the things for you.

Firsly, apply HypermediaAttribute to your controller actions

```C#
public class ValuesController : ApiController
{
    private static readonly Product[] Products = { 
            new Product() { Id = 1, Name = "Item1", Price = 2.99m},
            new Product() { Id = 2, Name = "Item2", Price = 3.99m}
        }; 
    [Hypermedia]
    public IEnumerable<Product> Get()
    {
        return Products;
    }
    [Hypermedia]
    public Product Get(int id)
    {
        return Products.First();
    }
}
```

Then implement `IHypermediaApiControllerConfigurator` interface. You can add this interface to your controller if you have empty constructor. Or you can create separate class to implement it.

```C#
public class ValuesControllerConfigurator : IHypermediaApiControllerConfigurator
{
    public void ConfigureHypermedia()
    {
        new HypermediaConfigurator<Product, ProductsController>(httpConfiguration)
            // Define rules for Get method receiving ID
            .For((model, controller) => controller.Get(model.Id))
            // Use Siren specification https://github.com/kevinswiber/siren
            .UseSirenSpecification()
                // A 'self' link will be added to the response.
                .Map((model, controller) => controller.Get(model.Id))
                    .AsSelfLink()
                // A 'parent' link will be added.
                .Map((model, controller) => controller.Get())
                    .AsParentLink()
                // A HTTM 'GET' action will be added.
                .Map((model, controller) => 
                        controller.Get(QueryParameter.Is<string>(), 
                            QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                    .AsAction()
                // A link to prodcut will be added. Instead od Id name will be used
                .Map((model, controller) => controller.Get(model.Name))
                // Post action will be added
                .Map((model, controller) => controller.Post(model))
                // Put action will be added
                .Map((model, controller) => controller.Put(model.Id, model))
                // Delete action will be added
                .Map((model, controller) => controller.Delete(model.Id))
                // MapReference will insert a link to ProductDetailsController.GetByProductId
                // using current product id
                .MapReference<ProductDetailsController>((model, referencedController) => 
                        referencedController.GetByProductId(model.Id))
                    .AsLink()
                // Add Procduct details as entities https://github.com/kevinswiber/siren#entities-1
                // Each object from ProductDetailsFromModel collection will be handled against
                // rules applied for ProductDetailsController.GetByProductId(int)
                .MapEmbeddedEntity<Models.ProductDetails, ProductDetailsController>(model => 
                        model.ProductDetailsFromModel,(model, controller) => 
                        controller.GetByProductId(model.Id))
                
             .For((model, controller) => controller.Get(model.Name))
                .UseSirenSpecification()
                .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                    .AsLink()
                .Map((model, controller) => controller.Get())
                    .AsParentLink()
                .Map((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                    .AsAction()
                .Map((model, controller) => controller.Get(model.Id))
                .Map((model, controller) => controller.Get(model.Name))
                    .AsSelfLink()
                .Map((model, controller) => controller.Post(model))
                .Map((model, controller) => controller.Put(model.Id, model))
                .Map((model, controller) => controller.Delete(model.Id))
                
            .For((model, controller) => controller.Get())
                .UseSirenSpecification()
                .Map((model, controller) => controller.Get())
                    .AsSelfLink()
                .Map((model, controller) => controller.Get(model.Id))
                .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                    .AsAction()
    
            .For((model, controller) => controller.Post(model))
                .UseSirenSpecification()
                .Map((model, controller) => controller.Get(model.Id))
                .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
            
            .For((model, controller) => controller.Get(QueryParameter.Is<string>(), QueryParameter.Is<int>(), QueryParameter.Is<int>()))
                .UseSirenSpecification()
                .Map((model, controller) => controller.Get())
                    .AsSelfLink()
                .MapReference<ProductDetailsController>((model, referencedController) => referencedController.GetByProductId(model.Id))
                    .AsAction()
    
        .Configure();
    }
}
```
Then call `InitializeHypermedia` method when you configure WebAPI. This will automatically discover all `IHypermediaApiControllerConfigurator` implementations and invoke `ConfigureHypermedia` method on it.

```C#
public class WebApiApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();

        GlobalConfiguration.Configure(WebApiConfig.Register);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        GlobalConfiguration.Configure(config => config.InitializeHypermedia());
    }
}
```

After doing that your GET `http://localhost/api/product/1` request you will yield the following response:

```json
[
{
    "properties": {
		"Id": 1,
		"Name": "Item1",
		"Price": 2.99
	},
	"links": [{
		"rel": ["self"],
		"href": "api/Product/1"
	},
	{
		"rel": ["parent", "__query"],
		"href": "api/Product"
	},
	{
		"rel": ["get_product_by_name"],
		"href": "api/Product/Item1"
	},
	{
		"rel": ["get_productdetails_by_id"],
		"href": "api/Product/1/Details"
	}],
	"actions": [{
		"name": "query_product_by_query_skip_limit",
		"class": ["__query"],
		"method": "GET",
		"href": "api/Product?query=:query&skip=:skip&limit=:limit",
		"fields": [{
			"name": "query"
		},
		{
			"name": "skip"
		},
		{
			"name": "limit"
		}]
	},
	{
		"name": "create-product",
		"method": "POST",
		"href": "api/Product",
		"type": "application/x-www-form-urlencoded",
		"fields": [{
			"name": "Id",
			"value": "1"
		},
		{
			"name": "Name",
			"value": "Item1"
		},
		{
			"name": "Price",
			"value": "2.99"
		}]
	},
	{
		"name": "put_by_id_product",
		"method": "PUT",
		"href": "api/Product/1",
		"fields": [{
			"name": "Id",
			"value": "1"
		},
		{
			"name": "Name",
			"value": "Item1"
		},
		{
			"name": "Price",
			"value": "2.99"
		}]
	},
	{
		"name": "delete_by_id",
		"method": "DELETE",
		"href": "api/Product/1"
	}],
	"entities": [{
		"properties": {
			"Id": 1,
			"ProductId": 1,
			"Details": "D1"
		},
		"links": [{
			"rel": ["get_productdetails_by_id"],
			"href": "api/Product/1/Details"
		}],
		"actions": [{
			"name": "post_by_value",
			"method": "POST",
			"href": "api/ProductDetails",
			"type": "application/x-www-form-urlencoded",
			"fields": [{
				"name": "Id",
				"value": "1"
			},
			{
				"name": "ProductId",
				"value": "1"
			},
			{
				"name": "Details",
				"value": "D1"
			}]
		},
		{
			"name": "put_by_id_value",
			"method": "PUT",
			"href": "api/ProductDetails/1",
			"fields": [{
				"name": "Id",
				"value": "1"
			},
			{
				"name": "ProductId",
				"value": "1"
			},
			{
				"name": "Details",
				"value": "D1"
			}]
		},
		{
			"name": "delete_by_id",
			"method": "DELETE",
			"href": "api/ProductDetails/1"
		}]
	}]
}]
```
Also there is [AngularJS Siren Provider](https://github.com/yury-sannikov/angular-hateoas-siren) which works in conjunction with NHateoas.

> **Tip:**   You can see rel or class marked as `__query`. This is a hint for [AngularJS Siren Provider](https://github.com/yury-sannikov/angular-hateoas-siren) to pick right $resource method.



## More information

 - [Siren](https://github.com/kevinswiber/siren) is considered as a main hypermedia format for future development
 - [Collection+JSON](http://amundsen.com/media-types/collection/) is another format I'm working with
 - [angular-hateoas](https://github.com/jmarquis/angular-hateoas) will be used as a sample consuming this library
 - [Building Hypermedia Web APIs with ASP.NET Web API](http://msdn.microsoft.com/en-us/magazine/jj883957.aspx)
 - [Presentation: From REST to HATEOAS](http://www.smartjava.org/content/presentation-rest-hateoas)
 - [WebApi Hal](https://github.com/JakeGinnivan/WebApi.Hal)
 - [A RESTful Hypermedia API in Three Easy Steps](http://www.amundsen.com/blog/archives/1041)
