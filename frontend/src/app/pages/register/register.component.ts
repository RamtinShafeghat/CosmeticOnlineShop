import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  fullName = '';
  email = '';
  phone = '';
  password = '';
  readonly busy = signal(false);
  readonly error = signal<string | null>(null);

  submit(): void {
    this.busy.set(true);
    this.error.set(null);
    this.auth
      .register({
        fullName: this.fullName.trim(),
        email: this.email.trim(),
        phone: this.phone.trim(),
        password: this.password
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          void this.router.navigate(['/account/orders']);
        },
        error: (err) => {
          this.busy.set(false);
          this.error.set(err?.error?.message || 'register.failed');
        }
      });
  }
}
