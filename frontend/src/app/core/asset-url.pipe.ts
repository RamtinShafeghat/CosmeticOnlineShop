import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../environments/environment';

@Pipe({ name: 'assetUrl', standalone: true })
export class AssetUrlPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) {
      return '';
    }
    if (value.startsWith('http://') || value.startsWith('https://') || value.startsWith('blob:')) {
      return value;
    }
    return `${environment.assetBaseUrl}${value}`;
  }
}
