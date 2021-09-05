import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { componentkeys } from 'src/environments/componentKeys';
import { environment } from 'src/environments/environment';
import { IComponent } from '../Core/Interfaces/Components/IComponent';
import { ISharedData } from '../Core/Interfaces/Components/ISharedData';
import { IApiResponse } from '../Core/Interfaces/IApiResponse';

@Injectable({
  providedIn: 'root'
})
export class PagesService {
  baseApiUrl = environment.baseApiUrl;
  comKeys = componentkeys;

  constructor(private http: HttpClient) {
    this.getAddRequestHeader();
  }

  currentLanguage: string;
  headers: HttpHeaders;
  getAddRequestHeader(){
    this.currentLanguage = localStorage.getItem('lang');
    this.headers = new HttpHeaders();
    this.headers = this.headers.set('accept-language', this.currentLanguage);
  }

  getSharedData(){
    return this.http.get<ISharedData>(this.baseApiUrl + 'Component/SharedData', {headers: this.headers});
  }
  
  getAboutBannarPhotos(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'Component/' + this.comKeys.aboutusBannerPhoto, {headers: this.headers});
  }

  getAboutUsWelcomMessage(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'Component/' + this.comKeys.aboutusWelcomMessage, {headers: this.headers});
  }

  getAboutUsTestimonial(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'Component/' + this.comKeys.aboutusTestimonial, {headers: this.headers});
  }

  getAboutUsOurTeams(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'Component/' + this.comKeys.aboutusOurTeams, {headers: this.headers});
  }
  getWebsiteSpecialServices(){
    return this.http.get<IComponent[]>(this.baseApiUrl + 'Component/' + this.comKeys.sharedSpecialServices, {headers: this.headers});
  }

  SendVisitorMessage(values: any) {
    return this.http.post(this.baseApiUrl + 'PageContact/visitorMsg', values, {headers: this.headers}).pipe(
      map((response: IApiResponse) => {
        return response;
      })
    );
  }
}
