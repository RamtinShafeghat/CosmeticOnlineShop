import { Injectable, computed, signal } from '@angular/core';
import { CartItem, Product } from '../models/shop.models';

const STORAGE_KEY = 'velora-cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly itemsSignal = signal<CartItem[]>(this.readStorage());

  readonly items = this.itemsSignal.asReadonly();
  readonly count = computed(() =>
    this.itemsSignal().reduce((sum, item) => sum + item.quantity, 0)
  );
  readonly subtotal = computed(() =>
    this.itemsSignal().reduce(
      (sum, item) => sum + item.product.price * item.quantity,
      0
    )
  );
  readonly shipping = computed(() => {
    const subtotal = this.subtotal();
    if (subtotal === 0 || subtotal >= 75) {
      return 0;
    }
    return 6.95;
  });
  readonly total = computed(() => this.subtotal() + this.shipping());

  add(product: Product, quantity = 1): void {
    const items = [...this.itemsSignal()];
    const existing = items.find((item) => item.product.id === product.id);

    if (existing) {
      existing.quantity = Math.min(existing.quantity + quantity, product.stock);
    } else {
      items.push({ product, quantity: Math.min(quantity, product.stock) });
    }

    this.persist(items);
  }

  updateQuantity(productId: number, quantity: number): void {
    const items = this.itemsSignal()
      .map((item) =>
        item.product.id === productId
          ? {
              ...item,
              quantity: Math.max(1, Math.min(quantity, item.product.stock))
            }
          : item
      )
      .filter((item) => item.quantity > 0);

    this.persist(items);
  }

  remove(productId: number): void {
    this.persist(this.itemsSignal().filter((item) => item.product.id !== productId));
  }

  clear(): void {
    this.persist([]);
  }

  private persist(items: CartItem[]): void {
    this.itemsSignal.set(items);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  }

  private readStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as CartItem[]) : [];
    } catch {
      return [];
    }
  }
}
