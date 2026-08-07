import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CarouselSlide } from '../models/shop.models';

@Injectable({ providedIn: 'root' })
export class CarouselService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/carousel-slides`;

  getSlides(): Observable<CarouselSlide[]> {
    return this.http.get<CarouselSlide[]>(this.baseUrl);
  }
}
