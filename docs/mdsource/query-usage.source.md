# Query Usage


## Arguments

The arguments supported are `ids`, `where`, `orderBy` , `skip`, and `take`.

Arguments are executed in that order.


### Ids

Queries entities by id. Currently the only supported identity member (property or field) name is `Id`.


#### Supported Types

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid), [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean), [Float](https://docs.microsoft.com/en-us/dotnet/api/system.float), [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte), [DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime), [DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset), [Decimal](https://docs.microsoft.com/en-us/dotnet/api/system.decimal), [Int16](https://docs.microsoft.com/en-us/dotnet/api/system.int16), [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32), [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64), [UInt16](https://docs.microsoft.com/en-us/dotnet/api/system.uint16), [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32), and [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64).


#### Single

```graphql
{
  entities (ids: "1")
  {
    property
  }
}
```


#### Multiple

```graphql
{
  entities (ids: ["1", "2"])
  {
    property
  }
}
```


### Where

Where statements are [and'ed](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-and-operator) together and executed in order.


#### Property Path

All where statements require a `path`. This is a full path to a, possible nested, property. Eg a property at the root level could be `Address`, while a nested property could be `Address.Street`. No null checking of nested values is done.


#### Supported Types

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid), [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean), [Float](https://docs.microsoft.com/en-us/dotnet/api/system.float), [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte), [DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime), [DateOnly](https://docs.microsoft.com/en-us/dotnet/api/system.dateonly), [TimeOnly](https://docs.microsoft.com/en-us/dotnet/api/system.timeonly), [DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset), [Decimal](https://docs.microsoft.com/en-us/dotnet/api/system.decimal), [Int16](https://docs.microsoft.com/en-us/dotnet/api/system.int16), [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32), [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64), [UInt16](https://docs.microsoft.com/en-us/dotnet/api/system.uint16), [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32), [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64), and [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum).


##### TimeOnly query

```
{
  timeEntities (where: {path: 'Property', comparison: equal, value: '10:11 AM'})
  {
    id
  }
}
```


##### DateOnly query

```
{
  dateEntities (where: {path: 'Property', comparison: equal, value: '2020-10-1'})
  {
    id
  }
}
```


#### Supported Comparisons

 * `equal`: (the default value if `comparison` is omitted)
 * `notEqual`: (the default value if `comparison` is omitted)
 * `greaterThan`
 * `greaterThanOrEqual`
 * `lessThan`
 * `lessThanOrEqual`:
 * `contains`: Only works with `string`
 * `startsWith`: Only works with `string`
 * `endsWith`: Only works with `string`
 * `in`: Check if a member existing in a given collection of values
 * `notIn`: Negation of in operator (**Deprecated**, use `negate` property with `in` operator instead)
 * `like`: Performs a SQL Like by using `EF.Functions.Like`

Case of comparison names are ignored. So, for example, `EndsWith`, `endsWith`, and `endswith` are  allowed.


#### Single

Single where statements can be expressed:

```graphql
{
  entities
  (where: {
    path: "Property",
    comparison: "equal",
    value: "the value"})
  {
    property
  }
}
```


#### Where In

```graphql
{
  testEntities
  (where: {
    path: "Property",
    comparison: "in",
    value: ["Value1", "Value2"]})
  {
    property
  }
}
```


#### Multiple Expressions and Expression Grouping

Expressions in the same logical grouping can be expressed together with a connector, on the preceeding where expression:

* `and`: (default if no connector provided)
* `or`

When trying to logically group expressions, provide a Where expression with only the `groupedExpressions` property.

Multiple where statements with a logical grouping can be expressed:

```graphql
{
  entities
  (where:
    [
      {path: "Property", comparison: "startsWith", value: "Valu"},
      {
        groupedExpressions: [
          {path: "Property", comparison: "endsWith", value: "ue", connector: "or"},
          {path: "Property", comparison: "endsWith", value: "id"}
        ]
      }
    ]
  )
  {
    property
  }
}
```

The above expression written as a logical statement would be:

```csharp
Property.startsWith("value") && (Property.endsWith("ue") || Property.endsWith("id"))
```


#### Query Negation

To negate any expression, including `groupedExpressions`, provide the `negate` property with true (Default is false).

Example:

```graphql
{
  entities
  (where:
    [
      {path: "Property", comparison: "startsWith", value: "Valu", negate: true},
      {
        negate: true,
        groupedExpressions: [
          {path: "Property", comparison: "endsWith", value: "ue", connector: "or"},
          {path: "Property", comparison: "endsWith", value: "id"}
        ]
      }
    ]
  )
  {
    property
  }
}
```


#### Querying List Members

A common query function is constraining a master set by some property of the detail list. For example to filter all orders by line items for a specific product.

**Note:** This only constrains the master list and doesn't affect the detail list, re-query the detail list with the same query to achieve this effect.

To query a list member graph property, use parenthesis (`[]`) to wrap the property. Example:

```graphql
{
  entities
  (where:
    [
      {
        path: "ListProperty[Property]",
        comparison: "startsWith",
        value: "Valu"
      }
    ]
  )
  {
    property
  }
}
```

Or:

```graphql
{
  entities
  (where:
    [
      {
        path: "ListProperty[Property.AnotherProperty]",
        comparison: "startsWith",
        value: "Valu"
      }
    ]
  )
  {
    property
  }
}
```


#### Null

Null can be expressed by omitting the `value`:

```graphql
{
  entities
  (where: {path: "Property", comparison: "equal"})
  {
    property
  }
}
```


### OrderBy


#### Ascending

```graphql
{
  entities (orderBy: {path: "Property"})
  {
    property
  }
}
```


#### Descending

```graphql
{
  entities (orderBy: {path: "Property", descending: true})
  {
    property
  }
}
```


### Take

[Queryable.Take](https://msdn.microsoft.com/en-us/library/bb300906(v=vs.110).aspx) or [Enumerable.Take](https://msdn.microsoft.com/en-us/library/bb503062.aspx) can be used as follows:

```graphql
{
  entities (take: 1)
  {
    property
  }
}
```


### Skip

[Queryable.Skip](https://msdn.microsoft.com/en-us/library/bb357513.aspx) or [Enumerable.Skip](https://msdn.microsoft.com/en-us/library/bb358985.aspx) can be used as follows:

```graphql
{
  entities (skip: 1)
  {
    property
  }
}
```
