import { Component, Input, OnInit } from '@angular/core';
import { IComponent } from 'src/app/Core/Interfaces/Components/IComponent';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.scss']
})
export class ServicesComponent implements OnInit {

  @Input() specilService: IComponent[];

  constructor() { }

  ngOnInit(): void {
  }
}
