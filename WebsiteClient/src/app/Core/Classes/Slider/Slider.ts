import { SliderStrings } from "./SliderStrings";

export class Slider{
    id: number;
    creatorId: string;
    createdDate: Date;
    pageName: string;
    component: string;
    componentName: string;
    sequencing: number;
    default: boolean;
    iconClass: string;
    newSign: boolean;
    published: boolean;
    startPublishDate: Date;
    endPublishDate: Date;
    sliderStrings: SliderStrings[];
}