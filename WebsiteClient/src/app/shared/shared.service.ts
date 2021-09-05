import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { componentkeys } from 'src/environments/componentKeys';
import { environment } from 'src/environments/environment';
import { IComponent } from '../Core/Interfaces/Components/IComponent';
import { ISharedData } from '../Core/Interfaces/Components/ISharedData';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  comKeys = componentkeys;
  baseApiUrl = environment.baseApiUrl;



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
}
