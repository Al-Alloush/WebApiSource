import { Component, OnInit, Input } from '@angular/core';
import { Slider } from 'src/app/Core/Classes/Slider/Slider';
import { HomeSlider } from '../../../shared/data/slider';

@Component({
  selector: 'app-slider',
  templateUrl: './slider.component.html',
  styleUrls: ['./slider.component.scss']
})
export class SliderComponent implements OnInit {

  constructor() { }
  
  @Input() sliders: Slider[];
  @Input() class: string;
  @Input() textClass: string;
  @Input() category: string;
  @Input() buttonText: string;
  @Input() buttonClass: string;

  public HomeSliderConfig: any = HomeSlider;

  ngOnInit(): void {
  }

}
