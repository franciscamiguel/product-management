import { render, screen } from '@testing-library/angular';
import { of } from 'rxjs';
import { CatalogPageComponent } from './catalog-page.component';
import { LoadingService } from '../../core/services/loading.service';
import { ProductsStore } from '../../core/services/products.store';

describe('CatalogPageComponent', () => {
  it('should render catalog title', async () => {
    const loadingStub = {
      isLoading: () => false
    };

    const storeStub = {
      pageInfo: () => ({ page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }),
      products: () => [],
      loading: () => false,
      error: () => null,
      load$: jest.fn(() => of({ items: [], page: 1, pageSize: 10, totalItems: 0, totalPages: 0 })),
      create$: jest.fn(() => of(null)),
      update$: jest.fn(() => of(null)),
      inactivate$: jest.fn(() => of(undefined))
    };

    await render(CatalogPageComponent, {
      providers: [
        { provide: LoadingService, useValue: loadingStub },
        { provide: ProductsStore, useValue: storeStub }
      ]
    });

    expect(screen.getByText('Listagem de Produtos')).toBeTruthy();
  });
});
