import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateOrderRequest, Order } from '../models/shop.models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/orders`;

  create(order: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, order);
  }

  getById(id: number, token?: string | null): Observable<Order> {
    let params = new HttpParams();
    if (token) {
      params = params.set('token', token);
    }
    return this.http.get<Order>(`${this.baseUrl}/${id}`, { params });
  }
}
