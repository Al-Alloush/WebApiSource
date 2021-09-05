import { Component, OnInit } from '@angular/core';
import { response } from 'express';
import { Slider } from '../Core/Classes/Slider/Slider';
import { SliderStrings } from '../Core/Classes/Slider/SliderStrings';
import { IComponent } from '../Core/Interfaces/Components/IComponent';
import { Product } from '../shared/classes/product';
import { ProductSlider } from '../shared/data/slider';
import { ProductService } from '../shared/services/product.service';
import { HomeService } from './home.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public products: Product[] = [];
  public productCollections: any[] = [];
  
  constructor(private homeService: HomeService , public productService: ProductService) {
    this.productService.getProducts.subscribe(response => {
      this.products = response.filter(item => item.type == 'fashion');
      // Get Product Collection
      this.products.filter((item) => {
        item.collection.filter((collection) => {
          const index = this.productCollections.indexOf(collection);
          if (index === -1) this.productCollections.push(collection);
        })
      })
    });
  }



  public ProductSliderConfig: any = ProductSlider;

// if delete it with get an Error with lider width when change the wedth
  sliders: Slider[] = [{
    id: null,
    creatorId: null,
    createdDate: null,
    pageName: null,
    component: null,
    componentName: null,
    sequencing: null,
    default: null,
    iconClass: null,
    newSign: null,
    published: null,
    startPublishDate: null,
    endPublishDate: null,
    sliderStrings: [{
      title: null,
      subtitle: null,
      shortDescription: null,
      buttonText: null,
      link: null,
      photoUrl: null,
      languageId: null
    }]
  }];


  // title: '',  supTitle: '', photoUrl: '', link: '', details: '' 
  // Collection banner
  collections: IComponent[] = null;
  parallaxs: IComponent[] = null;
  specilService: IComponent[] = null;
  ourParents: IComponent[] = null;
  logo: IComponent[] = null;

  // Blog
  public blog = [{
    image: 'assets/images/blog/1.jpg',
    date: '25 January 2018',
    title: 'Lorem ipsum dolor sit consectetur adipiscing elit,',
    by: 'John Dio'
  }, {
    image: 'assets/images/blog/2.jpg',
    date: '26 January 2018',
    title: 'Lorem ipsum dolor sit consectetur adipiscing elit,',
    by: 'John Dio'
  }, {
    image: 'assets/images/blog/3.jpg',
    date: '27 January 2018',
    title: 'Lorem ipsum dolor sit consectetur adipiscing elit,',
    by: 'John Dio'
  }, {
    image: 'assets/images/blog/4.jpg',
    date: '28 January 2018',
    title: 'Lorem ipsum dolor sit consectetur adipiscing elit,',
    by: 'John Dio'
  }];


  ngOnInit(): void {
    this.setHomeSlider();
    this.setShopCollection();
    this.setHomeParallax();
    this.sethomeSpecialServices();
    this.setHomeOurParents();
  }

  // get Home Slider
  setHomeSlider(){
    this.homeService.getHomeSlider().subscribe( response => {
      console.log(response);
      this.sliders = response;
    }, error => {
      this.sliders = null;
      console.log(error);
    });
  }
  setShopCollection(){
    this.homeService.getShopCollection().subscribe( response => {
      this.collections = response;
    }, error => {
      console.log(error);
    });
  }
  setHomeParallax(){
    this.homeService.gethomeParallax().subscribe( response => {
      this.parallaxs = response;
    }, error => {
      console.log(error);
    });
  }

  sethomeSpecialServices(){
    this.homeService.gethomeSpecialServices().subscribe( response => {
      this.specilService = response;
    }, error => {
      console.log(error);
    });
  }

  setHomeOurParents(){
    this.homeService.getHomeOurParents().subscribe( response => {
      this.ourParents = response;
    }, error => {
      console.log(error);
    });
  }


  // Product Tab collection
  getCollectionProducts(collection) {
    return this.products.filter((item) => {
      if (item.collection.find(i => i === collection)) {
        return item
      }
    })
  }
  
}
