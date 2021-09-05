import { Component, OnInit, Injectable, PLATFORM_ID, Inject, Input } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { ProductService } from "../../services/product.service";
import { Product } from "../../classes/product";
import { environment } from 'src/environments/environment';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  public products: Product[] = [];
  public search: boolean = false;
  
  @Input() languages: any[] = null;

  public currencies = [{
    name: 'Euro',
    currency: 'EUR',
    price: 0.90 // price of euro
  }, {
    name: 'Rupees',
    currency: 'INR',
    price: 70.93 // price of inr
  }, {
    name: 'Pound',
    currency: 'GBP',
    price: 0.78 // price of euro
  }, {
    name: 'Dollar',
    currency: 'USD',
    price: 1 // price of usd
  }];

  defaultLanguageCode = environment.defaultLanguageCode;

  // tslint:disable-next-line: ban-types
  constructor(@Inject(PLATFORM_ID) private platformId: Object,
              @Inject(DOCUMENT) private document: Document,
              private translate: TranslateService,
              public productService: ProductService) {
    this.productService.cartItems.subscribe(response => this.products = response);
  }

  currentLanguage: string;
  currentLanguageDir: string;

  ngOnInit(): void {
    this.currentLanguage = localStorage.getItem('lang');
    this.currentLanguageDir = localStorage.getItem('langDir');
    if (this.currentLanguage !== null){
      this.changeLanguage(this.currentLanguage);
    }else{
      this.changeLanguage(this.defaultLanguageCode);
    }
  }

  searchToggle(){
    this.search = !this.search;
  }

  changeLanguage(code){
    if (isPlatformBrowser(this.platformId)) {
      this.translate.use(code);
      if(code == 'ar') {
        document.body.classList.remove('ltr');
        document.body.classList.add('rtl');
      } else {
        document.body.classList.remove('rtl');
        document.body.classList.add('ltr');
      }
      if (this.currentLanguage !== code){
        this.document.defaultView.location.reload();
      }
      // stor language in localStor
      localStorage.setItem('lang', code);
    }
  }

  get getTotal(): Observable<number> {
    return this.productService.cartTotalAmount();
  }

  removeItem(product: any) {
    this.productService.removeCartItem(product);
  }

  changeCurrency(currency: any) {
    this.productService.Currency = currency;
  }

}
