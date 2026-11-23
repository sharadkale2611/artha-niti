import { Component, input } from '@angular/core';

@Component({
  selector: 'app-department-card',
  imports: [],
  templateUrl: './department-card.html',
  styleUrl: './department-card.css'
})
export class DepartmentCard {
  icon = input.required<string>();
  title = input.required<string>();
}