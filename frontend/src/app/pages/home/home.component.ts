import {
  Component,
  OnDestroy,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Product } from '../../core/models/shop.models';
import { ProductService } from '../../core/services/product.service';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

interface HeroSlide {
  image: string;
  alt: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, ProductCardComponent, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private timerId: ReturnType<typeof setInterval> | null = null;

  readonly slides: HeroSlide[] = [
    {
      image:
        'https://images.unsplash.com/photo-1596462502278-27bfdd403348?auto=format&fit=crop&w=1800&q=80',
      alt: 'Soft blush cosmetics still life'
    },
    {
      image:
        'https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?auto=format&fit=crop&w=1800&q=80',
      alt: 'Makeup brushes and beauty products'
    },
    {
      image:
        'https://images.unsplash.com/photo-1571781926291-c77df43ee830?auto=format&fit=crop&w=1800&q=80',
      alt: 'Skincare bottles on a calm surface'
    },
    {
      image:
        'https://images.unsplash.com/photo-1556228720-195a672e8a03?auto=format&fit=crop&w=1800&q=80',
      alt: 'Cream textures and spa ritual'
    }
  ];

  readonly activeSlide = signal(0);
  readonly featured = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly error = signal(false);

  ngOnInit(): void {
    this.startCarousel();
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
    this.activeSlide.update((current) => (current + 1) % this.slides.length);
  }

  prevSlide(): void {
    this.activeSlide.update(
      (current) => (current - 1 + this.slides.length) % this.slides.length
    );
  }

  private startCarousel(): void {
    this.stopCarousel();
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
