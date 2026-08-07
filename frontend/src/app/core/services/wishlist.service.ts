import { HttpClient } from '@angular/common/http';
import { Injectable, computed, effect, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../models/shop.models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly baseUrl = `${environment.apiUrl}/account/wishlist`;

  private readonly itemsSignal = signal<Product[]>([]);
  private readonly loadedSignal = signal(false);
  private readonly idsSignal = computed(() => new Set(this.itemsSignal().map((p) => p.id)));

  readonly items = this.itemsSignal.asReadonly();
  readonly loaded = this.loadedSignal.asReadonly();
  readonly count = computed(() => this.itemsSignal().length);

  constructor() {
    effect(() => {
      if (this.auth.isAuthenticated()) {
        this.refresh();
      } else {
        this.itemsSignal.set([]);
        this.loadedSignal.set(false);
      }
    });
  }

  has(productId: number): boolean {
    return this.idsSignal().has(productId);
  }

  refresh(): void {
    this.http.get<Product[]>(this.baseUrl).subscribe({
      next: (products) => {
        this.itemsSignal.set(products);
        this.loadedSignal.set(true);
      },
      error: () => this.loadedSignal.set(true)
    });
  }

  add(product: Product): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${product.id}`, null).pipe(
      tap(() => {
        if (!this.has(product.id)) {
          this.itemsSignal.update((items) => [product, ...items]);
        }
      })
    );
  }

  remove(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${productId}`).pipe(
      tap(() => {
        this.itemsSignal.update((items) => items.filter((p) => p.id !== productId));
      })
    );
  }

  toggle(product: Product): Observable<void> {
    return this.has(product.id) ? this.remove(product.id) : this.add(product);
  }
}
