import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  email = '';
  password = '';
  readonly busy = signal(false);
  readonly error = signal<string | null>(null);

  submit(): void {
    this.busy.set(true);
    this.error.set(null);
    this.auth.login({ email: this.email.trim(), password: this.password }).subscribe({
      next: () => {
        this.busy.set(false);
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/account/orders';
        void this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        this.busy.set(false);
        this.error.set(err?.error?.message || 'login.failed');
      }
    });
  }
}
