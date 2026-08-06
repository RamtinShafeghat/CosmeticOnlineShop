import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { CreateOrderRequest, CustomerAddress } from '../../core/models/shop.models';
import { AccountService } from '../../core/services/account.service';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, RouterLink, CurrencyPipe, TranslatePipe],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit {
  readonly cart = inject(CartService);
  readonly auth = inject(AuthService);
  readonly i18n = inject(LanguageService);
  private readonly orderService = inject(OrderService);
  private readonly account = inject(AccountService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly errorKey = signal<'checkout.bagEmpty' | 'checkout.failed' | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly addresses = signal<CustomerAddress[]>([]);
  selectedAddressId: number | '' = '';
  saveAddress = false;
  addressLabel = 'Home';

  form = {
    customerName: '',
    email: '',
    phone: '',
    shippingAddress: '',
    city: '',
    postalCode: ''
  };

  ngOnInit(): void {
    const profile = this.auth.profile();
    if (profile) {
      this.form.customerName = profile.fullName || '';
      this.form.email = profile.email || '';
      this.form.phone = profile.phone || '';
    }

    if (this.auth.isAuthenticated()) {
      this.account.getAddresses().subscribe({
        next: (items) => {
          this.addresses.set(items);
          const preferred = items.find((a) => a.isDefault) ?? items[0];
          if (preferred) {
            this.selectedAddressId = preferred.id;
            this.applyAddress(preferred);
          }
        }
      });
    }
  }

  onAddressSelect(rawId: number | '' | null): void {
    if (rawId === '' || rawId == null) {
      this.selectedAddressId = '';
      return;
    }
    this.selectedAddressId = rawId;
    const address = this.addresses().find((item) => item.id === rawId);
    if (address) {
      this.applyAddress(address);
    }
  }

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
      saveAddress: this.auth.isAuthenticated() && this.saveAddress,
      addressLabel: this.addressLabel,
      items: this.cart.items().map((item) => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    this.orderService.create(payload).subscribe({
      next: (order) => {
        this.cart.clear();
        this.submitting.set(false);
        void this.router.navigate(['/order', order.id], {
          queryParams: { token: order.publicToken }
        });
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

  private applyAddress(address: CustomerAddress): void {
    this.form.customerName = address.fullName;
    this.form.phone = address.phone;
    this.form.shippingAddress = address.line1;
    this.form.city = address.city;
    this.form.postalCode = address.postalCode;
  }
}
