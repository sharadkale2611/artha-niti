import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SectionTitle } from '../../shared/components/section-title/section-title';
import { NoticeCard } from '../../shared/components/notice-card/notice-card';

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    SectionTitle,
    NoticeCard
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

}