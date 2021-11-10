import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserService } from '../_services/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private service: UserService, private toastr: ToastrService) {}

  canActivate(): Observable<boolean> {
    return this.service.currentUser$.pipe(
      map((user) => {
        if (user) {
          return true;
        }
        this.toastr.error('You are not Authorized for This Request');
      })
    );
  }
}
