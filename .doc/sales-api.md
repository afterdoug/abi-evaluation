[Back to README](../README.md)

# Sales API

## Endpoints

### GET /sales/{id}
- **Description**: Retrieve a specific sale by ID
- **Path Parameters**:
  - `id`: Sale ID (GUID)
- **Response**: 
  ```json
  {
    "id": "guid",
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
  ```

### POST /sales
- **Description**: Create a new sale
- **Request Body**:
  ```json
  {
    "saleNumber": "string",
    "saleDate": "string (ISO 8601 format)",
    "customer": "string",
    "branch": "string",
    "items": [
      {
        "productId": "integer",
        "productName": "string",
        "quantity": "decimal",
        "unitPrice": "decimal",
        "discount": "decimal"
      }
    ]
  }
  ```
- **Response**: 
  ```json
  {
    "success": true,
    "message": "Sale created successfully",
    "data": {
      "id": "guid",
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
      "isCancelled": false,
      "createdAt": "string (ISO 8601 format)",
      "updatedAt": "string (ISO 8601 format)"
    }
  }
  ```

### PUT /sales/{id}
- **Description**: Update an existing sale
- **Path Parameters**:
  - `id`: Sale ID (GUID)
- **Request Body**:
  ```json
  {
    "id": "guid",
    "saleNumber": "string",
    "saleDate": "string (ISO 8601 format)",
    "customer": "string",
    "branch": "string",
    "items": [
      {
        "productId": "integer",
        "productName": "string",
        "quantity": "decimal",
        "unitPrice": "decimal",
        "discount": "decimal"
      }
    ]
  }
  ```
- **Response**: 
  ```json
  {
    "id": "guid",
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
  ```

### PATCH /sales/{id}/cancel
- **Description**: Cancel a specific sale
- **Path Parameters**:
  - `id`: Sale ID (GUID)
- **Response**: 
  ```json
  {
    "success": true,
    "message": "Sale cancelled successfully"
  }
  ```

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
        "id": "guid",
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
  ```

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./users-api.md">Previous: Users API</a>
  <a href="./products-api.md">Next: Products API</a>
</div>