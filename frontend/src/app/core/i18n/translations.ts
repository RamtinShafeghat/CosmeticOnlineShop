export type Lang = 'en' | 'fa';

export type TranslationKey =
  | 'nav.home'
  | 'nav.shop'
  | 'nav.bag'
  | 'lang.en'
  | 'lang.fa'
  | 'home.headline'
  | 'home.lead'
  | 'home.ctaShop'
  | 'home.ctaSkincare'
  | 'home.featuredTitle'
  | 'home.featuredLead'
  | 'home.ritualTitle'
  | 'home.ritualLead'
  | 'home.browseAll'
  | 'home.loading'
  | 'home.error'
  | 'shop.title'
  | 'shop.lead'
  | 'shop.searchPlaceholder'
  | 'shop.search'
  | 'shop.all'
  | 'shop.loading'
  | 'shop.error'
  | 'shop.empty'
  | 'shop.categoryMissing'
  | 'product.addToBag'
  | 'product.viewBag'
  | 'product.added'
  | 'product.qty'
  | 'product.skinType'
  | 'product.inStock'
  | 'product.loading'
  | 'product.notFound'
  | 'product.backToShop'
  | 'cart.title'
  | 'cart.empty'
  | 'cart.continue'
  | 'cart.summary'
  | 'cart.subtotal'
  | 'cart.shipping'
  | 'cart.free'
  | 'cart.total'
  | 'cart.note'
  | 'cart.checkout'
  | 'cart.remove'
  | 'cart.qty'
  | 'checkout.title'
  | 'checkout.empty'
  | 'checkout.continue'
  | 'checkout.fullName'
  | 'checkout.email'
  | 'checkout.phone'
  | 'checkout.address'
  | 'checkout.city'
  | 'checkout.postal'
  | 'checkout.placeOrder'
  | 'checkout.placing'
  | 'checkout.summary'
  | 'checkout.bagEmpty'
  | 'checkout.failed'
  | 'order.loading'
  | 'order.notFound'
  | 'order.back'
  | 'order.thanks'
  | 'order.confirmed'
  | 'order.receipt'
  | 'order.status'
  | 'order.placed'
  | 'order.shipTo'
  | 'order.shipping'
  | 'order.free'
  | 'order.total'
  | 'order.continue'
  | 'order.status.Confirmed'
  | 'order.status.Pending'
  | 'footer.tagline'
  | 'footer.shopAll'
  | 'footer.skincare'
  | 'footer.makeup'
  | 'footer.legal'
  | 'skin.All'
  | 'skin.Dry'
  | 'skin.Normal'
  | 'skin.Sensitive';

export const TRANSLATIONS: Record<Lang, Record<TranslationKey, string>> = {
  en: {
    'nav.home': 'Home',
    'nav.shop': 'Shop',
    'nav.bag': 'Bag',
    'lang.en': 'EN',
    'lang.fa': 'FA',
    'home.headline': 'Skin that feels quietly luminous.',
    'home.lead':
      'Refined skincare and soft-color makeup crafted for everyday radiance.',
    'home.ctaShop': 'Shop the collection',
    'home.ctaSkincare': 'Explore skincare',
    'home.featuredTitle': 'Featured rituals',
    'home.featuredLead':
      'A curated edit of Velora essentials—serums, soft color, and scent.',
    'home.ritualTitle': 'Beauty, unhurried.',
    'home.ritualLead':
      'Velora formulas favor clean textures, soft finishes, and ingredients that earn their place in a daily ritual.',
    'home.browseAll': 'Browse all products',
    'home.loading': 'Loading the collection…',
    'home.error': 'Unable to load featured products. Is the API running?',
    'shop.title': 'Shop',
    'shop.lead': 'Discover Velora skincare, makeup, fragrance, and body care.',
    'shop.searchPlaceholder': 'Search products',
    'shop.search': 'Search',
    'shop.all': 'All',
    'shop.loading': 'Loading products…',
    'shop.error': 'Unable to load products. Is the API running?',
    'shop.empty': 'No products match your filters.',
    'shop.categoryMissing': 'Category not found.',
    'product.addToBag': 'Add to bag',
    'product.viewBag': 'View bag',
    'product.added': 'Added to your bag.',
    'product.qty': 'Qty',
    'product.skinType': 'Skin type',
    'product.inStock': 'In stock',
    'product.loading': 'Loading product…',
    'product.notFound': 'Product not found.',
    'product.backToShop': 'Back to shop',
    'cart.title': 'Your bag',
    'cart.empty': 'Your bag is empty.',
    'cart.continue': 'Continue shopping',
    'cart.summary': 'Summary',
    'cart.subtotal': 'Subtotal',
    'cart.shipping': 'Shipping',
    'cart.free': 'Free',
    'cart.total': 'Total',
    'cart.note': 'Free shipping on orders over $75.',
    'cart.checkout': 'Checkout',
    'cart.remove': 'Remove',
    'cart.qty': 'Qty',
    'checkout.title': 'Checkout',
    'checkout.empty': 'Your bag is empty.',
    'checkout.continue': 'Continue shopping',
    'checkout.fullName': 'Full name',
    'checkout.email': 'Email',
    'checkout.phone': 'Phone',
    'checkout.address': 'Shipping address',
    'checkout.city': 'City',
    'checkout.postal': 'Postal code',
    'checkout.placeOrder': 'Place order',
    'checkout.placing': 'Placing order…',
    'checkout.summary': 'Order summary',
    'checkout.bagEmpty': 'Your bag is empty.',
    'checkout.failed': 'Could not place the order. Please try again.',
    'order.loading': 'Loading order…',
    'order.notFound': 'Order not found.',
    'order.back': 'Back to shop',
    'order.thanks': 'Thank you',
    'order.confirmed': 'Order #{id} confirmed',
    'order.receipt':
      'We’ve emailed a receipt to {email}. Your Velora order is on its way to being prepared.',
    'order.status': 'Status',
    'order.placed': 'Placed',
    'order.shipTo': 'Ship to',
    'order.shipping': 'Shipping',
    'order.free': 'Free',
    'order.total': 'Total',
    'order.continue': 'Continue shopping',
    'order.status.Confirmed': 'Confirmed',
    'order.status.Pending': 'Pending',
    'footer.tagline': 'Quiet luxury for everyday skin rituals.',
    'footer.shopAll': 'Shop all',
    'footer.skincare': 'Skincare',
    'footer.makeup': 'Makeup',
    'footer.legal': '© {year} Velora Beauty',
    'skin.All': 'All',
    'skin.Dry': 'Dry',
    'skin.Normal': 'Normal',
    'skin.Sensitive': 'Sensitive'
  },
  fa: {
    'nav.home': 'خانه',
    'nav.shop': 'فروشگاه',
    'nav.bag': 'سبد',
    'lang.en': 'EN',
    'lang.fa': 'فا',
    'home.headline': 'پوستی که آرام می‌درخشد.',
    'home.lead':
      'مراقبت پوست ظریف و آرایش رنگ ملایم برای درخشش روزانه.',
    'home.ctaShop': 'مشاهده مجموعه',
    'home.ctaSkincare': 'کشف مراقبت پوست',
    'home.featuredTitle': 'آیین‌های ویژه',
    'home.featuredLead':
      'گزیده‌ای از ضروریات ولورا—سرم‌ها، رنگ‌های نرم و عطر.',
    'home.ritualTitle': 'زیبایی بی‌شتاب.',
    'home.ritualLead':
      'فرمول‌های ولورا بافت‌های پاک، پایان نرم و موادی را ترجیح می‌دهند که جای خود را در آیین روزانه پیدا کنند.',
    'home.browseAll': 'مشاهده همه محصولات',
    'home.loading': 'در حال بارگذاری مجموعه…',
    'home.error': 'بارگذاری محصولات ویژه ممکن نشد. آیا API در حال اجراست؟',
    'shop.title': 'فروشگاه',
    'shop.lead': 'کشف مراقبت پوست، آرایش، عطر و مراقبت بدن ولورا.',
    'shop.searchPlaceholder': 'جستجوی محصولات',
    'shop.search': 'جستجو',
    'shop.all': 'همه',
    'shop.loading': 'در حال بارگذاری محصولات…',
    'shop.error': 'بارگذاری محصولات ممکن نشد. آیا API در حال اجراست؟',
    'shop.empty': 'محصولی با این فیلترها یافت نشد.',
    'shop.categoryMissing': 'دسته‌بندی پیدا نشد.',
    'product.addToBag': 'افزودن به سبد',
    'product.viewBag': 'مشاهده سبد',
    'product.added': 'به سبد اضافه شد.',
    'product.qty': 'تعداد',
    'product.skinType': 'نوع پوست',
    'product.inStock': 'موجودی',
    'product.loading': 'در حال بارگذاری محصول…',
    'product.notFound': 'محصول پیدا نشد.',
    'product.backToShop': 'بازگشت به فروشگاه',
    'cart.title': 'سبد خرید شما',
    'cart.empty': 'سبد خرید شما خالی است.',
    'cart.continue': 'ادامه خرید',
    'cart.summary': 'خلاصه',
    'cart.subtotal': 'جمع جزء',
    'cart.shipping': 'ارسال',
    'cart.free': 'رایگان',
    'cart.total': 'جمع کل',
    'cart.note': 'ارسال رایگان برای سفارش‌های بالای ۷۵ دلار.',
    'cart.checkout': 'تسویه حساب',
    'cart.remove': 'حذف',
    'cart.qty': 'تعداد',
    'checkout.title': 'تسویه حساب',
    'checkout.empty': 'سبد خرید شما خالی است.',
    'checkout.continue': 'ادامه خرید',
    'checkout.fullName': 'نام کامل',
    'checkout.email': 'ایمیل',
    'checkout.phone': 'تلفن',
    'checkout.address': 'آدرس ارسال',
    'checkout.city': 'شهر',
    'checkout.postal': 'کد پستی',
    'checkout.placeOrder': 'ثبت سفارش',
    'checkout.placing': 'در حال ثبت سفارش…',
    'checkout.summary': 'خلاصه سفارش',
    'checkout.bagEmpty': 'سبد خرید شما خالی است.',
    'checkout.failed': 'ثبت سفارش ممکن نشد. لطفاً دوباره تلاش کنید.',
    'order.loading': 'در حال بارگذاری سفارش…',
    'order.notFound': 'سفارش پیدا نشد.',
    'order.back': 'بازگشت به فروشگاه',
    'order.thanks': 'متشکریم',
    'order.confirmed': 'سفارش #{id} تأیید شد',
    'order.receipt':
      'رسید به {email} ایمیل شد. سفارش ولورای شما در حال آماده‌سازی است.',
    'order.status': 'وضعیت',
    'order.placed': 'زمان ثبت',
    'order.shipTo': 'ارسال به',
    'order.shipping': 'ارسال',
    'order.free': 'رایگان',
    'order.total': 'جمع کل',
    'order.continue': 'ادامه خرید',
    'order.status.Confirmed': 'تأیید شده',
    'order.status.Pending': 'در انتظار',
    'footer.tagline': 'لوکس آرام برای آیین‌های روزانه پوست.',
    'footer.shopAll': 'همه محصولات',
    'footer.skincare': 'مراقبت پوست',
    'footer.makeup': 'آرایش',
    'footer.legal': '© {year} زیبایی ولورا',
    'skin.All': 'همه',
    'skin.Dry': 'خشک',
    'skin.Normal': 'معمولی',
    'skin.Sensitive': 'حساس'
  }
};
