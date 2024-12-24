Yes, that’s a reasonable way to think about the role of an **`IEndpointRouteBuilder`** in ASP.NET Core, though it's a bit abstracted from a direct dictionary-like implementation. Let’s break it down.

---

### **`IEndpointRouteBuilder` Overview**
The `IEndpointRouteBuilder` interface is used in ASP.NET Core to define routes and their associated logic during application startup. When you call methods like `MapGet`, `MapPost`, or `MapControllerRoute`, you are essentially adding route definitions and their handlers (logic) to an internal **data structure** managed by the framework.

---

### **What Does `IEndpointRouteBuilder` Do?**
1. **Collects Routes**:
   - It is responsible for collecting all the route definitions during the application's startup phase. These routes are represented as **`Endpoint`** objects.
   - Each `Endpoint` ties a route pattern (e.g., `/home`, `/products/{id}`) to a delegate or controller action that will handle requests matching that route.

2. **Builds the Routing Table**:
   - The collected `Endpoint` objects are used to construct a **routing table** (internally managed by the framework). This routing table is consulted at runtime to match incoming HTTP requests to the appropriate route handler.

3. **Maps Logic to Routes**:
   - When you define a route, you typically associate it with a delegate (such as a `RequestDelegate` or a controller action) that will execute when a request matches that route.

---

### **Internals: Dictionary-Like Behavior**
While it doesn't literally build a `Dictionary`, the conceptual analogy holds:

- **Key**: The route pattern or template (e.g., `/home`, `/products/{id}`).
- **Value**: The logic to execute for that route (e.g., a delegate, a controller action).

The internal data structures are more complex than a simple dictionary:
- They optimize for efficient route matching (e.g., using tree structures for patterns, tokenization, etc.).
- They also account for features like constraints, priorities, and fallback routes.

---

### **Example**
Consider the following route setup:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello, World!");
app.MapGet("/products/{id:int}", (int id) => $"Product ID: {id}");
app.MapPost("/products", (Product product) => $"Created Product: {product.Name}");

app.Run();
```

What happens internally:
1. **Route Patterns**:
   - `/` → Maps to a `RequestDelegate` that returns "Hello, World!".
   - `/products/{id:int}` → Maps to a `RequestDelegate` that handles the route logic for an integer ID.
   - `/products` (POST) → Maps to a `RequestDelegate` that handles creating a product.

2. **Routing Table**:
   - The `IEndpointRouteBuilder` collects these route definitions and builds an efficient internal structure.

3. **Matching Requests**:
   - At runtime, when a request is made, the routing middleware uses this structure to quickly find and execute the appropriate logic.

---

### **Why Is This More Than a Dictionary?**
A simple dictionary lookup doesn’t handle the complexities of routing:
- **Pattern Matching**:
  - Routes may contain templates (e.g., `/products/{id}`), constraints (e.g., `:int`), and defaults.
- **Verb Matching**:
  - Different HTTP verbs (GET, POST, PUT) can have the same route pattern but map to different handlers.
- **Prioritization**:
  - Routes are matched in a specific order based on their definitions and constraints.
- **Fallbacks**:
  - Special routes like fallback routes are considered when no other route matches.

---

### **Key Points**
1. **`IEndpointRouteBuilder`** helps define routes and the logic associated with them.
2. Internally, it creates and stores `Endpoint` objects that represent the routing logic.
3. While conceptually similar to a dictionary, the actual routing table is more complex and optimized for performance and flexibility.
4. At runtime, the **routing middleware** uses the routing table to resolve incoming requests to the appropriate `Endpoint`.

---

In summary, thinking of `IEndpointRouteBuilder` as building a dictionary-like structure is a useful mental model for understanding its purpose. However, keep in mind that the actual implementation is more sophisticated to handle the complexities of modern web routing.