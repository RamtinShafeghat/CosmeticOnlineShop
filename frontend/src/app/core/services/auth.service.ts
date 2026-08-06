import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CustomerAuthResponse,
  CustomerLoginRequest,
  CustomerProfile,
  CustomerRegisterRequest
} from '../models/shop.models';

const TOKEN_KEY = 'velora-customer-token';
const PROFILE_KEY = 'velora-customer-profile';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly tokenSignal = signal<string | null>(this.readToken());
  private readonly profileSignal = signal<CustomerProfile | null>(this.readProfile());

  readonly token = this.tokenSignal.asReadonly();
  readonly profile = this.profileSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.tokenSignal());

  register(payload: CustomerRegisterRequest): Observable<CustomerAuthResponse> {
    return this.http
      .post<CustomerAuthResponse>(`${this.baseUrl}/register`, payload)
      .pipe(tap((response) => this.setSession(response)));
  }

  login(payload: CustomerLoginRequest): Observable<CustomerAuthResponse> {
    return this.http
      .post<CustomerAuthResponse>(`${this.baseUrl}/login`, payload)
      .pipe(tap((response) => this.setSession(response)));
  }

  setSession(response: CustomerAuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    const profile: CustomerProfile = {
      email: response.email,
      fullName: response.fullName,
      phone: response.phone,
      expiresAtUtc: response.expiresAtUtc
    };
    localStorage.setItem(PROFILE_KEY, JSON.stringify(profile));
    this.tokenSignal.set(response.token);
    this.profileSignal.set(profile);
  }

  logout(navigateToLogin = true): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(PROFILE_KEY);
    this.tokenSignal.set(null);
    this.profileSignal.set(null);
    if (navigateToLogin) {
      void this.router.navigate(['/login']);
    }
  }

  private readToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  private readProfile(): CustomerProfile | null {
    try {
      const raw = localStorage.getItem(PROFILE_KEY);
      return raw ? (JSON.parse(raw) as CustomerProfile) : null;
    } catch {
      return null;
    }
  }
}
