import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(
    public service: UserService,
    private route: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    this.service.login(this.model).subscribe(
      (response) => {
        console.log(response);
        this.route.navigateByUrl('/members');
      },
      (err) => {
        console.log(err);
        this.toastr.error(err.error);
      }
    );
  }

  logout() {
    this.service.logout();
    this.route.navigateByUrl('/');
  }
}
