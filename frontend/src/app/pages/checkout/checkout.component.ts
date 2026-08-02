import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { CreateOrderRequest } from '../../core/models/shop.models';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, RouterLink, CurrencyPipe, TranslatePipe],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent {
  readonly cart = inject(CartService);
  readonly i18n = inject(LanguageService);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly errorKey = signal<'checkout.bagEmpty' | 'checkout.failed' | null>(null);
  readonly errorMessage = signal<string | null>(null);

  form = {
    customerName: '',
    email: '',
    phone: '',
    shippingAddress: '',
    city: '',
    postalCode: ''
  };

  submit(): void {
    if (this.cart.items().length === 0) {
      this.errorKey.set('checkout.bagEmpty');
      return;
    }

    this.submitting.set(true);
    this.errorKey.set(null);
    this.errorMessage.set(null);

    const payload: CreateOrderRequest = {
      ...this.form,
      items: this.cart.items().map((item) => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    this.orderService.create(payload).subscribe({
      next: (order) => {
        this.cart.clear();
        this.submitting.set(false);
        void this.router.navigate(['/order', order.id]);
      },
      error: (err) => {
        this.submitting.set(false);
        if (err?.error?.message) {
          this.errorMessage.set(err.error.message);
        } else {
          this.errorKey.set('checkout.failed');
        }
      }
    });
  }
}
