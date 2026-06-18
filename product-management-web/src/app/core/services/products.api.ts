import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateProductRequest,
  PagedResult,
  Product,
  ProductQuery,
  UpdateProductRequest
} from '../models/product.models';

@Injectable({ providedIn: 'root' })
export class ProductsApi {
  constructor(private readonly httpClient: HttpClient) {}

  list(query: ProductQuery): Observable<PagedResult<Product>> {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);

    if (query.name) {
      params = params.set('name', query.name);
    }

    if (query.category) {
      params = params.set('category', query.category);
    }

    if (query.status) {
      params = params.set('status', query.status);
    }

    return this.httpClient.get<PagedResult<Product>>(`${environment.apiBaseUrl}/products`, { params });
  }

  create(payload: CreateProductRequest): Observable<Product> {
    return this.httpClient.post<Product>(`${environment.apiBaseUrl}/products`, payload);
  }

  update(id: string, payload: UpdateProductRequest): Observable<Product> {
    return this.httpClient.put<Product>(`${environment.apiBaseUrl}/products/${id}`, payload);
  }

  inactivate(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${environment.apiBaseUrl}/products/${id}/inactivate`);
  }

  activate(id: string): Observable<void> {
    return this.httpClient.patch<void>(`${environment.apiBaseUrl}/products/${id}/activate`, {});
  }
}
