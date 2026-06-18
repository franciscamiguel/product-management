import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BehaviorSubject, EMPTY, Subject, catchError, debounceTime, distinctUntilChanged, filter, startWith, switchMap, tap } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LoadingService } from '../../core/services/loading.service';
import { ProductsStore } from '../../core/services/products.store';
import { Product, ProductQuery, ProductStatus, UpdateProductRequest } from '../../core/models/product.models';
import { ProductFormDialogComponent } from './product-form-dialog.component';

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe, DatePipe, MatButtonModule, MatProgressSpinnerModule, MatDialogModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CatalogPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialog = inject(MatDialog);
  private readonly refreshTrigger = new Subject<void>();

  readonly loadingService = inject(LoadingService);
  readonly productsStore = inject(ProductsStore);

  readonly currentPage = signal(1);
  readonly statusOptions: ProductStatus[] = ['Active', 'Inactive'];

  readonly filterForm = this.formBuilder.nonNullable.group({
    name: [''],
    category: [''],
    status: ['' as '' | ProductStatus],
    sortBy: ['createdAtUtc'],
    sortDirection: ['desc' as 'asc' | 'desc'],
    pageSize: [10]
  });

  readonly pageInfo = computed(() => this.productsStore.pageInfo());
  readonly products = computed(() => this.productsStore.products());
  readonly canGoPrevious = computed(() => this.pageInfo().page > 1);
  readonly canGoNext = computed(() => this.pageInfo().page < this.pageInfo().totalPages);

  constructor() {
    const filterChanges$ = this.filterForm.valueChanges.pipe(
      startWith(this.filterForm.getRawValue()),
      debounceTime(350),
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
      tap(() => this.currentPage.set(1))
    );

    const pageChanges$ = new BehaviorSubject<number>(this.currentPage());

    this.refreshTrigger
      .pipe(
        startWith(undefined),
        tap(() => pageChanges$.next(this.currentPage())),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();

    filterChanges$
      .pipe(
        switchMap(() => pageChanges$),
        switchMap(() => this.productsStore.load$(this.buildQuery())),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  openCreateModal(): void {
    this.dialog
      .open(ProductFormDialogComponent, {
        width: '760px',
        maxWidth: '96vw'
      })
      .afterClosed()
      .pipe(
        filter((payload: UpdateProductRequest | undefined): payload is UpdateProductRequest => !!payload),
        switchMap((payload) => this.productsStore.create$(payload)),
        filter((value) => value !== null),
        tap(() => this.refresh()),
        catchError(() => EMPTY),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  openEditModal(product: Product): void {
    this.dialog
      .open(ProductFormDialogComponent, {
        width: '760px',
        maxWidth: '96vw',
        data: { product }
      })
      .afterClosed()
      .pipe(
        filter((payload: UpdateProductRequest | undefined): payload is UpdateProductRequest => !!payload),
        switchMap((payload) => this.productsStore.update$(product.id, payload)),
        filter((value) => value !== null),
        tap(() => this.refresh()),
        catchError(() => EMPTY),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  inactivate(productId: string): void {
    this.productsStore
      .inactivate$(productId)
      .pipe(
        tap(() => this.refresh()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  activate(productId: string): void {
    this.productsStore
      .activate$(productId)
      .pipe(
        tap(() => this.refresh()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  toggleStatus(product: Product): void {
    if (product.status === 'Active') {
      this.inactivate(product.id);
      return;
    }

    this.activate(product.id);
  }

  goToNextPage(): void {
    if (!this.canGoNext()) {
      return;
    }

    this.currentPage.update((value) => value + 1);
    this.refresh();
  }

  goToPreviousPage(): void {
    if (!this.canGoPrevious()) {
      return;
    }

    this.currentPage.update((value) => value - 1);
    this.refresh();
  }

  private refresh(): void {
    this.refreshTrigger.next();
  }

  private buildQuery(): ProductQuery {
    const filters = this.filterForm.getRawValue();

    return {
      page: this.currentPage(),
      pageSize: Number(filters.pageSize) || 10,
      name: filters.name?.trim() || undefined,
      category: filters.category?.trim() || undefined,
      status: filters.status || undefined,
      sortBy: filters.sortBy || 'createdAtUtc',
      sortDirection: filters.sortDirection || 'desc'
    };
  }
}
