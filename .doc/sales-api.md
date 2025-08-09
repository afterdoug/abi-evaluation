# Sales API Documentation

[Back to README](../README.md)

## Sales

### GET /sales
- **Description**: Retrieve a list of all sales
- **Query Parameters**:
  - `_page` (optional): Page number for pagination (default: 1)
  - `_size` (optional): Number of items per page (default: 10)
  - `_order` (optional): Ordering of results (e.g., "saleDate desc, customer asc")
- **Response**: 
  ```json
  {
    "data": [
      {
        "id": "integer",
        "saleNumber": "string",
        "saleDate": "string (ISO 8601 format)",
        "customer": "string",
        "totalAmount": "decimal",
        "branch": "string",
        "items": [
          {
            "productId": "integer",
            "productName": "string",
            "quantity": "decimal",
            "unitPrice": "decimal",
            "discount": "decimal",
            "totalAmount": "decimal"
          }
        ],
        "isCancelled": "boolean",
        "createdAt": "string (ISO 8601 format)",
        "updatedAt": "string (ISO 8601 format)"
      }
    ],
    "totalItems": "integer",
    "currentPage": "integer",
    "totalPages": "integer"
  }