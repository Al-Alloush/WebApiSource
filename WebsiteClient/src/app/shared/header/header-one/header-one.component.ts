import { Component, OnInit, Input, HostListener } from '@angular/core';
import { IComponent } from 'src/app/Core/Interfaces/Components/IComponent';
import { ISharedData } from 'src/app/Core/Interfaces/Components/ISharedData';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-header-one',
  templateUrl: './header-one.component.html',
  styleUrls: ['./header-one.component.scss']
})
export class HeaderOneComponent implements OnInit {
  
  @Input() class: string;
  @Input() themeLogo: string = 'assets/images/icon/logo.png'; // Default Logo
  @Input() topbar: boolean = true; // Default True
  @Input() sticky: boolean = false; // Default false
  
  public stick: boolean = false;

  headerWelcomMessage: IComponent[] = [];
  footerWelcomMessage: IComponent[] = [];
  phoneNumber: IComponent[] = [];
  websiteAddress: IComponent[] = [];
  websiteEmail: IComponent[] = [];

  public sharedData: ISharedData;

  constructor(private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getSharedData();

  }

  getSharedData(){
    this.sharedService.getSharedData().subscribe( response =>{
      // console.log(response);
      this.sharedData = response;
    }, error => {
      console.log(error);
    });
  }


  // @HostListener Decorator
  @HostListener("window:scroll", [])
  onWindowScroll() {
    let number = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
  	if (number >= 150 && window.innerWidth > 400) { 
  	  this.stick = true;
  	} else {
  	  this.stick = false;
  	}
  }

}
