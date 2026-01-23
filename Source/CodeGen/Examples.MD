# TypeScript Generation Examples

## Complete Transformation Showcase

This document shows how C# models are transformed into TypeScript definitions.

---

## Example 1: Simple DTO

### C# Input
```csharp
namespace RetailPortal.Model.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }

    public enum UserRole
    {
        Guest = 0,
        Customer = 1,
        Administrator = 3
    }
}
```

### TypeScript Output
```typescript
declare module RetailPortal.Model.Dtos {
    interface IUserDto {
        id: string;                    // Guid → string
        firstName: string;              // PascalCase → camelCase
        lastName: string;
        email: string;
        role: RetailPortal.Model.Dtos.UserRole;
        createdAt: Date;                // DateTime → Date
        lastLoginAt: Date;              // DateTime? → Date (nullable)
        isActive: boolean;
    }

    export enum UserRole {
        Guest = 0,
        Customer = 1,
        Administrator = 3
    }
}
```

### Key Transformations
| C# | TypeScript | Note |
|----|------------|------|
| `class UserDto` | `interface IUserDto` | Added 'I' prefix |
| `FirstName` | `firstName` | PascalCase → camelCase |
| `Guid` | `string` | Type mapping |
| `DateTime` | `Date` | Type mapping |
| `DateTime?` | `Date` | Nullables preserved |
| `bool` | `boolean` | Type mapping |
| `enum UserRole` | `export enum UserRole` | Direct mapping |

---

## Example 2: Complex DTO with Collections

### C# Input
```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public ProductCategory Category { get; set; }
}
```

### TypeScript Output
```typescript
interface IProductDto {
    id: string;
    name: string;
    price: number;                          // decimal → number
    stock: number;                          // int → number
    tags: string[];                         // List<string> → string[]
    metadata: { [key: string]: string };    // Dictionary → index signature
    category: RetailPortal.Model.Dtos.ProductCategory;
}
```

### Type Mappings
| C# Type | TypeScript Type |
|---------|----------------|
| `Guid` | `string` |
| `string` | `string` |
| `decimal` | `number` |
| `int` | `number` |
| `long` | `number` |
| `double` | `number` |
| `float` | `number` |
| `bool` | `boolean` |
| `DateTime` | `Date` |
| `DateTime?` | `Date` |
| `List<T>` | `T[]` |
| `Dictionary<string, T>` | `{ [key: string]: T }` |
| `T[]` | `T[]` |

---

## Example 3: Nested Objects

### C# Input
```csharp
public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AddressDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
}
```

### TypeScript Output
```typescript
interface ICreateOrderRequest {
    userId: string;
    items: RetailPortal.Model.Dtos.IOrderItemDto[];  // Array of interfaces
    shippingAddress: RetailPortal.Model.Dtos.IAddressDto;  // Nested interface
    paymentMethod: RetailPortal.Model.Dtos.PaymentMethod;
}

interface IOrderItemDto {
    productId: string;
    quantity: number;
    unitPrice: number;
}

interface IAddressDto {
    street: string;
    city: string;
    state: string;
}
```

---

## Example 4: Generic Types

### C# Input
```csharp
public class PagedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
}

// Usage
public class UserListResponse : PagedResponse<UserDto> { }
```

### TypeScript Output
```typescript
interface IPagedResponse<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    hasNextPage: boolean;
}

// Usage in TypeScript
type UserListResponse = IPagedResponse<IUserDto>;
```

---

## Example 5: Enums

### C# Input
```csharp
public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Refunded = 5
}

[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Admin = 8
}
```

### TypeScript Output
```typescript
export enum OrderStatus {
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Refunded = 5
}

export enum Permissions {
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Admin = 8
}
```

**Note:** TypeScript enums preserve numeric values and can be used for bitwise operations just like C# `[Flags]` enums.

---

## Example 6: Real-World Usage in TypeScript

### API Service
```typescript
import { RetailPortal } from './generated-models';

type UserDto = RetailPortal.Model.Dtos.IUserDto;
type OrderResponse = RetailPortal.Model.Dtos.IOrderResponse;

class ApiService {
    async getUser(id: string): Promise<UserDto> {
        const response = await fetch(`/api/users/${id}`);
        return response.json();
    }

    async createOrder(request: RetailPortal.Model.Dtos.ICreateOrderRequest): Promise<OrderResponse> {
        const response = await fetch('/api/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request)
        });
        return response.json();
    }
}
```

### React Component
```typescript
import React, { useState, useEffect } from 'react';
import { RetailPortal } from './generated-models';

const { UserRole, OrderStatus } = RetailPortal.Model.Dtos;
type UserDto = RetailPortal.Model.Dtos.IUserDto;

const UserProfile: React.FC<{ userId: string }> = ({ userId }) => {
    const [user, setUser] = useState<UserDto | null>(null);

    useEffect(() => {
        fetch(`/api/users/${userId}`)
            .then(res => res.json())
            .then(setUser);
    }, [userId]);

    if (!user) return <div>Loading...</div>;

    return (
        <div>
            <h1>{user.firstName} {user.lastName}</h1>
            <p>Email: {user.email}</p>
            <p>Role: {UserRole[user.role]}</p>
            <p>Active: {user.isActive ? 'Yes' : 'No'}</p>
        </div>
    );
};
```

---

## Naming Convention Summary

### Properties
- **C#**: `PascalCase` (e.g., `FirstName`)
- **TypeScript**: `camelCase` (e.g., `firstName`)

### Interfaces
- **C#**: `UserDto` (class name)
- **TypeScript**: `IUserDto` (with 'I' prefix)

### Enums
- **C#**: `UserRole`
- **TypeScript**: `UserRole` (unchanged)

### Generic Type Parameters
- **C#**: `T`, `TKey`, `TValue`
- **TypeScript**: `T`, `TKey`, `TValue` (preserved)

---

## Benefits

- **Type Safety**: Compile-time checking in TypeScript
- **IntelliSense**: Full autocomplete in your IDE
- **Refactoring**: Changes in C# automatically propagate
- **Documentation**: Self-documenting API contracts
- **Consistency**: Same models on backend and frontend
- **Reduced Errors**: Catch mismatches before runtime
