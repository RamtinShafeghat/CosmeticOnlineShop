import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CustomerAddress,
  CustomerOrderListItem,
  Order,
  UpsertCustomerAddress
} from '../models/shop.models';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/account`;

  getOrders(): Observable<CustomerOrderListItem[]> {
    return this.http.get<CustomerOrderListItem[]>(`${this.baseUrl}/orders`);
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/orders/${id}`);
  }

  getAddresses(): Observable<CustomerAddress[]> {
    return this.http.get<CustomerAddress[]>(`${this.baseUrl}/addresses`);
  }

  createAddress(payload: UpsertCustomerAddress): Observable<CustomerAddress> {
    return this.http.post<CustomerAddress>(`${this.baseUrl}/addresses`, payload);
  }

  updateAddress(id: number, payload: UpsertCustomerAddress): Observable<CustomerAddress> {
    return this.http.put<CustomerAddress>(`${this.baseUrl}/addresses/${id}`, payload);
  }

  deleteAddress(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/addresses/${id}`);
  }
}
