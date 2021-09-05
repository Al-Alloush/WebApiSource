import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { map } from 'rxjs/operators';
import { IComponent } from 'src/app/Core/Interfaces/Components/IComponent';
import { ISharedData } from 'src/app/Core/Interfaces/Components/ISharedData';
import { PagesService } from '../../pages.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {

  visitorMsgForm: FormGroup;

  sharedData: ISharedData;

  constructor(private pagesService: PagesService) { }

  ngOnInit(): void {
    this.getSharedData();
    this.createVisitorMsgForm();
  }

  createVisitorMsgForm(){
    this.visitorMsgForm = new FormGroup({
      email: new FormControl('', [Validators.required , Validators.email]),
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      phoneNumber: new FormControl(''),
      visitorMessage: new FormControl('', Validators.required)
    });
  }

  onSubmitVisitorMsg(){
    console.log(this.visitorMsgForm.value);
    this.pagesService.SendVisitorMessage(this.visitorMsgForm.value).subscribe(response => {
      alert(response.responseMessage);
    });
  }

  getSharedData(){
    this.pagesService.getSharedData().subscribe( response =>{
      console.log(response);
      this.sharedData = response;
    }, error => {
      console.log(error);
    });
  }



  sendVisitorMessage(data){
    alert(data.firstName);
  }

}
