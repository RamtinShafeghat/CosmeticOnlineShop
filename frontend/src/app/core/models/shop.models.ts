export interface Category {
  id: number;
  name: string;
  slug: string;
  description: string;
  productCount: number;
}

export interface Product {
  id: number;
  name: string;
  slug: string;
  shortDescription: string;
  description?: string;
  price: number;
  imageUrl: string;
  brand: string;
  skinType: string;
  stock: number;
  isFeatured: boolean;
  categoryId: number;
  categoryName: string;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface CreateOrderItem {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerName: string;
  email: string;
  phone: string;
  shippingAddress: string;
  city: string;
  postalCode: string;
  items: CreateOrderItem[];
}

export interface OrderItem {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  customerName: string;
  email: string;
  phone: string;
  shippingAddress: string;
  city: string;
  postalCode: string;
  status: string;
  subtotal: number;
  shippingCost: number;
  total: number;
  createdAt: string;
  items: OrderItem[];
}
