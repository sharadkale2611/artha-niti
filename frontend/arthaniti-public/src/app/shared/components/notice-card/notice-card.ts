import { Component, input } from '@angular/core';

@Component({
  selector: 'app-notice-card',
  imports: [],
  templateUrl: './notice-card.html',
  styleUrl: './notice-card.css'
})
export class NoticeCard {
  date = input.required<string>();
  title = input.required<string>();
  description = input<string>('');
}