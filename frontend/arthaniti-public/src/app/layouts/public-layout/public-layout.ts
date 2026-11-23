import { Component } from '@angular/core';
import { Header } from '../../shared/components/header/header';
import { RouterOutlet } from '@angular/router';
import { MainNavbar } from '../../shared/components/main-navbar/main-navbar';
import { Footer } from '../../shared/components/footer/footer';

@Component({
  selector: 'app-public-layout',
imports: [
    RouterOutlet,
    Header,
    MainNavbar,
    Footer
  ],  templateUrl: './public-layout.html',
  styleUrl: './public-layout.css',
})
export class PublicLayout {}
