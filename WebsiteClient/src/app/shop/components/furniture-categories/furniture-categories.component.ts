import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-furniture-categories',
  templateUrl: './furniture-categories.component.html',
  styleUrls: ['./furniture-categories.component.scss']
})
export class FurnitureCategoriesComponent implements OnInit {
  public collapse: boolean = true;
  @Input() categories : string[];


  constructor() { }

  ngOnInit(): void {
  }

}
