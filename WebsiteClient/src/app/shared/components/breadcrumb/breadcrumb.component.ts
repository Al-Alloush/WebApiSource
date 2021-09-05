import { Component, OnInit, Input } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {

  @Input() title : string;
  @Input() breadcrumb : string;
  currentLanguage = environment.defaultLanguageCode;
  langDirection = 'ltr'

  constructor() {
  }

  ngOnInit() : void {  
    //get current language from local Storage
    const lang = localStorage.getItem('lang');
    if (lang !== null){
      if(lang ==='ar'){
        this.currentLanguage = lang;
        this.langDirection = 'rtl'
      }
    }
  }

}
