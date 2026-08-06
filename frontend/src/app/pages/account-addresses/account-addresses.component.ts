import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationKey } from '../../core/i18n/translations';
import { CustomerAddress, UpsertCustomerAddress } from '../../core/models/shop.models';
import { AccountService } from '../../core/services/account.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-account-addresses',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './account-addresses.component.html',
  styleUrl: './account-addresses.component.scss'
})
export class AccountAddressesComponent implements OnInit {
  private readonly account = inject(AccountService);
  private readonly auth = inject(AuthService);

  readonly addresses = signal<CustomerAddress[]>([]);
  readonly loading = signal(true);
  readonly error = signal<TranslationKey | string | null>(null);
  readonly saving = signal(false);
  readonly editingId = signal<number | null>(null);

  form: UpsertCustomerAddress = this.emptyForm();

  ngOnInit(): void {
    this.reload();
  }

  startCreate(): void {
    this.editingId.set(null);
    this.form = this.emptyForm();
  }

  startEdit(address: CustomerAddress): void {
    this.editingId.set(address.id);
    this.form = {
      label: address.label,
      fullName: address.fullName,
      phone: address.phone,
      line1: address.line1,
      city: address.city,
      postalCode: address.postalCode,
      isDefault: address.isDefault
    };
  }

  save(): void {
    this.saving.set(true);
    this.error.set(null);
    const id = this.editingId();
    const request$ =
      id == null
        ? this.account.createAddress(this.form)
        : this.account.updateAddress(id, this.form);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.startCreate();
        this.reload();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message || 'account.addressSaveFailed');
      }
    });
  }

  remove(address: CustomerAddress): void {
    if (!confirm(`Delete address "${address.label}"?`)) {
      return;
    }
    this.account.deleteAddress(address.id).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('account.addressDeleteFailed')
    });
  }

  private reload(): void {
    this.loading.set(true);
    this.account.getAddresses().subscribe({
      next: (items) => {
        this.addresses.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('account.addressesError');
        this.loading.set(false);
      }
    });
  }

  private emptyForm(): UpsertCustomerAddress {
    const profile = this.auth.profile();
    return {
      label: 'Home',
      fullName: profile?.fullName || '',
      phone: profile?.phone || '',
      line1: '',
      city: '',
      postalCode: '',
      isDefault: this.addresses().length === 0
    };
  }
}
