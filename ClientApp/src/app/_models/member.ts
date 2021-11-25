import { Photo } from './photo';

export interface Member {
  id: number;
  userName: string;
  photoUrl: string;
  age: number;
  knownAs: string;
  lastActive: Date;
  created: Date;
  gender: string;
  lookingFor: string;
  introduction: string;
  interests: string;
  city: string;
  country: string;
  photos: Photo[];
}
