import { Component, OnInit } from '@angular/core';
import { response } from 'express';
import { IComponent } from 'src/app/Core/Interfaces/Components/IComponent';
import { AboutBannerSlider, TeamSlider, TestimonialSlider } from '../../shared/data/slider';
import { PagesService } from '../pages.service';

@Component({
  selector: 'app-about-us',
  templateUrl: './about-us.component.html',
  styleUrls: ['./about-us.component.scss']
})
export class AboutUsComponent implements OnInit {

  specilServices: IComponent[] = [];
  welcomMessages: IComponent[] = [];
  teams: IComponent[] = [ { photoUrl: '', title: '', details: '' }];
  bannerPhotos: IComponent[] = [ { photoUrl: ''}];
  testimonial: IComponent[] = [ { photoUrl: '', title: '', details: '', description: '' }];
  public TeamSliderConfig: any = TeamSlider;
  public TestimonialSliderConfig: any = TestimonialSlider;
  public AboutBannerSliderConfig: any = AboutBannerSlider;

  constructor(private pagesService: PagesService) { }

  ngOnInit(): void {
    this.getAboutBannarPhotos();
    this.getAboutUsWelcomMessge();
    this.getAboutUsTestimonial();
    this.getAboutUsOurTeams();
    this.getWebsiteSpesialService();
  }


  getAboutBannarPhotos(){
    this.pagesService.getAboutBannarPhotos().subscribe(response => {
     this.bannerPhotos = response;
     // console.log(response);
    }, error =>{
      console.log(error);
    });
  }

 getAboutUsWelcomMessge(){
   this.pagesService.getAboutUsWelcomMessage().subscribe(response => {
    this.welcomMessages = response;
    // console.log(response);
   }, error =>{
     console.log(error);
   });
 }

 getAboutUsTestimonial(){
  this.pagesService.getAboutUsTestimonial().subscribe(response => {
   this.testimonial = response;
   // console.log(response);
  }, error =>{
    console.log(error);
  });
}


getAboutUsOurTeams(){
  this.pagesService.getAboutUsOurTeams().subscribe(response => {
   this.teams = response;
   // console.log(response);
  }, error =>{
    console.log(error);
  });
}
 getWebsiteSpesialService(){
   this.pagesService.getWebsiteSpecialServices().subscribe(response => {
     this.specilServices = response;
     // console.log(response);
   }, error => {
     console.log(error);
   });
 }

}
