import { Injectable, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AdminLoginResponse } from './models';

const TOKEN_KEY = 'velora-admin-token';
const PROFILE_KEY = 'velora-admin-profile';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenSignal = signal<string | null>(this.readToken());
  private readonly profileSignal = signal<Omit<AdminLoginResponse, 'token'> | null>(
    this.readProfile()
  );

  readonly token = this.tokenSignal.asReadonly();
  readonly profile = this.profileSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.tokenSignal());

  constructor(private readonly router: Router) {}

  setSession(response: AdminLoginResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    const profile = {
      email: response.email,
      displayName: response.displayName,
      expiresAtUtc: response.expiresAtUtc
    };
    localStorage.setItem(PROFILE_KEY, JSON.stringify(profile));
    this.tokenSignal.set(response.token);
    this.profileSignal.set(profile);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(PROFILE_KEY);
    this.tokenSignal.set(null);
    this.profileSignal.set(null);
    void this.router.navigate(['/login']);
  }

  private readToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  private readProfile(): Omit<AdminLoginResponse, 'token'> | null {
    try {
      const raw = localStorage.getItem(PROFILE_KEY);
      return raw ? (JSON.parse(raw) as Omit<AdminLoginResponse, 'token'>) : null;
    } catch {
      return null;
    }
  }
}
