import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-foods-categories',
  templateUrl: './foods-categories.component.html',
  styleUrls: ['./foods-categories.component.scss']
})
export class FoodsCategoriesComponent implements OnInit {
  public collapse: boolean = true;
  @Input() categories : string[];


  constructor() { }

  ngOnInit(): void {
  }


}
