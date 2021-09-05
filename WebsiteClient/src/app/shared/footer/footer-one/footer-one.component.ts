import { Component, OnInit, Input } from '@angular/core';
import { ISharedData } from 'src/app/Core/Interfaces/Components/ISharedData';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-footer-one',
  templateUrl: './footer-one.component.html',
  styleUrls: ['./footer-one.component.scss']
})
export class FooterOneComponent implements OnInit {

  @Input() class: string = 'footer-light' // Default class 
  @Input() themeLogo: string = 'assets/images/icon/logo.png' // Default Logo
  @Input() newsletter: boolean = true; // Default True

  public today: number = Date.now();


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

}
