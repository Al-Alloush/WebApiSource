import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { componentkeys } from 'src/environments/componentKeys';
import { environment } from 'src/environments/environment';
import { Slider } from '../Core/Classes/Slider/Slider';
import { IComponent } from '../Core/Interfaces/Components/IComponent';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  comKeys = componentkeys;

  baseApiUrl = environment.baseApiUrl;

  currentLanguage = null;

  headers: HttpHeaders;

  constructor(private http: HttpClient) {
    this.getAddRequestHeader();
  }

  getAddRequestHeader(){
    this.currentLanguage = localStorage.getItem('lang');
    this.headers = new HttpHeaders();
    this.headers = this.headers.set('accept-language', this.currentLanguage);
  }

  // ---------------------------  Home Page requests
  getHomeSlider(){
    return this.http.get<Slider[]>(this.baseApiUrl + 'PageComponentManage/slider/newOffers', { headers: this.headers });
  }

  getShopCollection(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'PageComponentManage/' + this.comKeys.homeCollection, { headers: this.headers });
  }

  gethomeParallax(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'PageComponentManage/' + this.comKeys.homeParallax, { headers: this.headers });
  }

  gethomeSpecialServices(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'PageComponentManage/' + this.comKeys.sharedSpecialServices, { headers: this.headers });
  }

  getHomeOurParents(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'PageComponentManage/' + this.comKeys.homeOurPartners, { headers: this.headers });
  }
  // END ---------------------------  Home Page requests
}
