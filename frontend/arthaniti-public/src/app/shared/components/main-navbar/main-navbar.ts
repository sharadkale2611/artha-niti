import { Component } from '@angular/core';
import {
  RouterLink,
  RouterLinkActive
} from '@angular/router';

@Component({
  selector: 'app-main-navbar',
  imports: [
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './main-navbar.html',
  styleUrl: './main-navbar.css'
})
export class MainNavbar {

}