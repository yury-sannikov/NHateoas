# NHateoas

> **Tip:**   Currently NHateoas is in a <!---prototying stage-->. We are considering different mediatype formats at this moment.

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
        new HypermediaConfigurator<Product, ValuesController>()
            .For((model, controller) => controller.Get())
                .Map((model, controller) => controller.Get(model.Id))
                .MapReference<ProductDetailsController>((model, referencedController) 
                    => referencedController.GetByProductId(model.Id))

            .For((model, controller) => controller.Get(model.Id))
                .Map((model, controller) => controller.Get())
                .Map((model, controller) 
                    => controller.Get(
                        QueryParameter.Is<string>(), 
                        QueryParameter.Is<int>(), 
                        QueryParameter.Is<int>()))
                .Map((model, controller) => controller.Get(model.Name))
                .Map((model, controller) => controller.Post(model))
                .Map((model, controller) => controller.Put(model.Id, model))
                .Map((model, controller) => controller.Delete(model.Id))
                .MapReference<ProductDetailsController>((model, referencedController) 
                    => referencedController.GetByProductId(model.Id))

             .For((model, controller) => controller.Get(model.Name))
                .MapReference<ProductDetailsController>((model, referencedController) 
                    => referencedController.GetByProductId(model.Id))
                .Map((model, controller) => controller.Get())
                .Map((model, controller) 
                    => controller.Get(
                        QueryParameter.Is<string>(), 
                        QueryParameter.Is<int>(), 
                        QueryParameter.Is<int>()))
                .Map((model, controller) => controller.Get(model.Id))
                .Map((model, controller) => controller.Get(model.Name))
                .Map((model, controller) => controller.Post(model))
                .Map((model, controller) => controller.Put(model.Id, model))
                .Map((model, controller) => controller.Delete(model.Id))

            .For((model, controller) => controller.Post(model))
                .Map((model, controller) => controller.Get(model.Id))
                .MapReference<ProductDetailsController>((model, referencedController) 
                    => referencedController.GetByProductId(model.Id))

        .Configure();
    }
}
```
Then call `InitializeHypermedia` method when you configure WebAPI. This will automatically discover all `IHypermediaApiControllerConfigurator` implementations and invoke `ConfigureHypermedia` method on it.

```C#
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute ...

        config.InitializeHypermedia();
    }
}
```

After doing that your GET request you will yield the following response:

```json
[
    {
        "Id":1,
        "Name":"Item1",
        "Price":2.99,
        "get_product_by_id":"api/Values/1",
        "get_productdetails_by_id":"api/Values/1/ProductDetails"    
    },
    {
        "Id":2,
        "Name":"Item2",
        "Price":3.99,"get_product_by_id":"api/Values/2",
        "get_productdetails_by_id":"api/Values/2/ProductDetails"
    }
]
```

> **Tip:**   Currently NHateoas is in a <!---prototying stage-->. We are considering different mediatype formats at this moment. We considering [Siren](https://github.com/kevinswiber/siren) as a main hypermedia format.


## More information

 - [Siren](https://github.com/kevinswiber/siren) is considered as a main hypermedia format for future development
 - [Collection+JSON](http://amundsen.com/media-types/collection/) is another format I'm working with
 - [angular-hateoas](https://github.com/jmarquis/angular-hateoas) will be used as a sample consuming this library
 - [Building Hypermedia Web APIs with ASP.NET Web API](http://msdn.microsoft.com/en-us/magazine/jj883957.aspx)
 - [Presentation: From REST to HATEOAS](http://www.smartjava.org/content/presentation-rest-hateoas)
 - [WebApi Hal](https://github.com/JakeGinnivan/WebApi.Hal)
 - [A RESTful Hypermedia API in Three Easy Steps](http://www.amundsen.com/blog/archives/1041)
