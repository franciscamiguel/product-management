import { Injectable, computed, signal } from '@angular/core';
import { Observable, catchError, finalize, of, tap } from 'rxjs';
import {
  CreateProductRequest,
  PagedResult,
  Product,
  ProductQuery,
  UpdateProductRequest
} from '../models/product.models';
import { ProductsApi } from './products.api';

@Injectable({ providedIn: 'root' })
export class ProductsStore {
  private readonly data = signal<PagedResult<Product>>({
    items: [],
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  });

  private readonly busy = signal(false);
  private readonly errorValue = signal<string | null>(null);

  readonly products = computed(() => this.data().items);
  readonly pageInfo = computed(() => this.data());
  readonly loading = computed(() => this.busy());
  readonly error = computed(() => this.errorValue());

  constructor(private readonly productsApi: ProductsApi) {}

  load$(query: ProductQuery): Observable<PagedResult<Product>> {
    this.busy.set(true);
    this.errorValue.set(null);

    return this.productsApi.list(query).pipe(
      tap((response) => this.data.set(response)),
      catchError(() => {
        this.errorValue.set('Não foi possível carregar o catálogo.');
        return of({
          items: [],
          page: query.page,
          pageSize: query.pageSize,
          totalItems: 0,
          totalPages: 0
        });
      }),
      finalize(() => this.busy.set(false))
    );
  }

  create$(payload: CreateProductRequest): Observable<Product | null> {
    return this.productsApi.create(payload).pipe(catchError(() => of(null)));
  }

  update$(id: string, payload: UpdateProductRequest): Observable<Product | null> {
    return this.productsApi.update(id, payload).pipe(catchError(() => of(null)));
  }

  inactivate$(id: string): Observable<void> {
    return this.productsApi.inactivate(id).pipe(catchError(() => of(undefined)));
  }

  activate$(id: string): Observable<void> {
    return this.productsApi.activate(id).pipe(catchError(() => of(undefined)));
  }
}
