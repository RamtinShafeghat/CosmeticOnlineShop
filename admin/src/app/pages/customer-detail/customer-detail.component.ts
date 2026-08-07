import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AdminCustomerDetail, UpdateCustomer } from '../../core/models';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, FormsModule, TranslatePipe],
  templateUrl: './customer-detail.component.html',
  styleUrl: './customer-detail.component.scss'
})
export class CustomerDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly i18n = inject(LanguageService);

  readonly customer = signal<AdminCustomerDetail | null>(null);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);
  readonly saving = signal(false);
  readonly deleting = signal(false);

  form: UpdateCustomer = { fullName: '', email: '', phone: '' };
  newPassword = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error.set(this.i18n.t('customerDetail.notFound'));
      return;
    }

    this.api.getCustomer(id).subscribe({
      next: (customer) => this.applyCustomer(customer),
      error: () => this.error.set(this.i18n.t('customerDetail.notFound'))
    });
  }

  save(): void {
    const current = this.customer();
    if (!current) {
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.message.set(null);

    const payload: UpdateCustomer = {
      ...this.form,
      newPassword: this.newPassword.trim() || undefined
    };

    this.api.updateCustomer(current.id, payload).subscribe({
      next: (customer) => {
        this.applyCustomer(customer);
        this.newPassword = '';
        this.saving.set(false);
        this.message.set(this.i18n.t('customerDetail.saved'));
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('customerDetail.saveFailed'));
      }
    });
  }

  remove(): void {
    const current = this.customer();
    if (!current) {
      return;
    }
    if (!confirm(this.i18n.t('customerDetail.deleteConfirm', { name: current.fullName }))) {
      return;
    }

    this.deleting.set(true);
    this.error.set(null);
    this.api.deleteCustomer(current.id).subscribe({
      next: () => void this.router.navigate(['/customers']),
      error: (err) => {
        this.deleting.set(false);
        this.error.set(err?.error?.message || this.i18n.t('customerDetail.deleteFailed'));
      }
    });
  }

  private applyCustomer(customer: AdminCustomerDetail): void {
    this.customer.set(customer);
    this.form = {
      fullName: customer.fullName,
      email: customer.email,
      phone: customer.phone
    };
  }
}
