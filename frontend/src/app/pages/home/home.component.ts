import {
  Component,
  OnDestroy,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { CarouselSlide, Product } from '../../core/models/shop.models';
import { CarouselService } from '../../core/services/carousel.service';
import { ProductService } from '../../core/services/product.service';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, ProductCardComponent, TranslatePipe, AssetUrlPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly carouselService = inject(CarouselService);
  private timerId: ReturnType<typeof setInterval> | null = null;

  readonly slides = signal<CarouselSlide[]>([]);
  readonly activeSlide = signal(0);
  readonly featured = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly error = signal(false);

  ngOnInit(): void {
    this.carouselService.getSlides().subscribe({
      next: (slides) => {
        this.slides.set(slides);
        this.startCarousel();
      },
      error: () => {
        // Hero copy still renders without a slideshow behind it.
      }
    });

    this.productService.getProducts({ featured: true }).subscribe({
      next: (products) => {
        this.featured.set(products.slice(0, 4));
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    this.stopCarousel();
  }

  goToSlide(index: number): void {
    this.activeSlide.set(index);
    this.restartCarousel();
  }

  nextSlide(): void {
    const total = this.slides().length;
    if (total === 0) {
      return;
    }
    this.activeSlide.update((current) => (current + 1) % total);
  }

  prevSlide(): void {
    const total = this.slides().length;
    if (total === 0) {
      return;
    }
    this.activeSlide.update((current) => (current - 1 + total) % total);
  }

  private startCarousel(): void {
    this.stopCarousel();
    if (this.slides().length <= 1) {
      return;
    }
    this.timerId = setInterval(() => this.nextSlide(), 5500);
  }

  private restartCarousel(): void {
    this.startCarousel();
  }

  private stopCarousel(): void {
    if (this.timerId) {
      clearInterval(this.timerId);
      this.timerId = null;
    }
  }
}
