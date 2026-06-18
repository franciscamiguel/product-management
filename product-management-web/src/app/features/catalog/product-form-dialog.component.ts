import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { Product, ProductStatus, UpdateProductRequest } from '../../core/models/product.models';

export interface ProductFormDialogData {
  product?: Product;
}

@Component({
  selector: 'app-product-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatButtonModule],
  templateUrl: './product-form-dialog.component.html',
  styleUrl: './product-form-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductFormDialogComponent {
  readonly statusOptions: ProductStatus[] = ['Active', 'Inactive'];
  readonly product = this.data?.product;
  readonly isEditMode = !!this.product;

  readonly form = this.formBuilder.nonNullable.group({
    name: [this.product?.name ?? '', [Validators.required, Validators.maxLength(120)]],
    sku: [this.product?.sku ?? '', [Validators.required, Validators.maxLength(50)]],
    description: [this.product?.description ?? '', [Validators.required, Validators.maxLength(1000)]],
    price: [this.product?.price ?? 0, [Validators.required, Validators.min(0.01)]],
    stockQuantity: [this.product?.stockQuantity ?? 0, [Validators.required, Validators.min(0)]],
    category: [this.product?.category ?? '', [Validators.required, Validators.maxLength(100)]],
    status: [this.product?.status ?? 'Active' as ProductStatus, Validators.required]
  });

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly dialogRef: MatDialogRef<ProductFormDialogComponent, UpdateProductRequest>,
    @Inject(MAT_DIALOG_DATA) public readonly data: ProductFormDialogData | null
  ) {}

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }

  close(): void {
    this.dialogRef.close();
  }
}
