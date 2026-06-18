export type ProductStatus = 'Active' | 'Inactive';

export interface Product {
  id: string;
  name: string;
  sku: string;
  description: string;
  price: number;
  stockQuantity: number;
  category: string;
  status: ProductStatus;
  createdAtUtc: string;
  updatedAtUtc?: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface ProductQuery {
  page: number;
  pageSize: number;
  name?: string;
  category?: string;
  status?: ProductStatus;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  description: string;
  price: number;
  stockQuantity: number;
  category: string;
  status: ProductStatus;
}

export type UpdateProductRequest = CreateProductRequest;
