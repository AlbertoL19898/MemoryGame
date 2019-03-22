using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Draw;
using System.Threading;

namespace esDrawMemory
{
    class Program
    {
        const int WIDTH_CARD = 70;
        const int HEIGHT_CARD = 120;
        const int NUM_CARD = 16;
        const int NUM_COL = 4;
        const float DEFAULT_CLICK_COUNTDOWN=1f;
        
        enum Colore
        {
            Rosso,
            Verde,
            Blu,
            Giallo,
            Arancione,
            Celeste,
            Bianco,
            Fucsia
        }

        struct Position
        {
            public int X;
            public int Y;
        }

        struct Color
        {
            public byte r;
            public byte g;
            public byte b;
        }

        struct Cards
        {
            public int Number;
            public bool Show;
            public Color color;
            public Position position;
        }

        struct Deck
        {
            public Cards[] Cards;
            public int NumCol;
            public Position position;
        }

        static void DrawPixel(Window window, int x, int y, byte r, byte g, byte b)
        {
            if (x < 0 || x >= window.width) return;
            if (y < 0 || y >= window.height) return;
            window.bitmap[((y * window.width) + x) * 3] = r;
            window.bitmap[(((y * window.width) + x) * 3) + 1] = g;
            window.bitmap[(((y * window.width) + x) * 3) + 2] = b;
        }

        static void DrawFullRect(Window window, int x, int y, int width, int height, byte r, byte g, byte b)
        {
            for (int i = y; i < y + height; i++)
            {
                for (int j = x; j < x + width; j++)
                {
                    DrawPixel(window, j, i, r, g, b);
                }
            }
        }

        static Color InitColor(ref Cards c, Colore Co)
        {
            switch (Co)
            {
                case Colore.Rosso:
                    c.color.r = 255;
                    c.color.g = 0;
                    c.color.b = 0;
                    break;
                case Colore.Verde:
                    c.color.r = 0;
                    c.color.g = 255;
                    c.color.b = 0;
                    break;
                case Colore.Blu:
                    c.color.r = 0;
                    c.color.g = 0;
                    c.color.b = 255;
                    break;
                case Colore.Giallo:
                    c.color.r = 250;
                    c.color.g = 255;
                    c.color.b = 0;
                    break;
                case Colore.Arancione:
                    c.color.r = 255;
                    c.color.g = 157;
                    c.color.b = 0;
                    break;
                case Colore.Celeste:
                    c.color.r = 0;
                    c.color.g = 255;
                    c.color.b = 255;
                    break;
                case Colore.Bianco:
                    c.color.r = 255;
                    c.color.g = 255;
                    c.color.b = 255;
                    break;
                case Colore.Fucsia:
                    c.color.r = 255;
                    c.color.g = 0;
                    c.color.b = 255;
                    break;
                default:
                    break;
            }
            return c.color;
        }


        static void InitDeck(out Deck deck, int numCards, int numCol)
        {
            deck.Cards = new Cards[numCards];
            deck.NumCol = numCol;
            deck.position.X = 100;
            deck.position.Y = 100;
            
            int halfsize = numCards / 2;

            for (int i = 0; i < halfsize; i++)
            {
                deck.Cards[i].Number = i;
                deck.Cards[i].Show = false;
                deck.Cards[i].color = InitColor(ref deck.Cards[i], (Colore)i);
                deck.Cards[i].position.X=0;
                deck.Cards[i].position.Y=0;
                
                deck.Cards[i + halfsize].Number = i;
                deck.Cards[i + halfsize].Show = false;
                deck.Cards[i + halfsize].color = InitColor(ref deck.Cards[i], (Colore)i);
                deck.Cards[i].position.X=0;
                deck.Cards[i].position.Y=0;
            }
        }  
        
        static void Shuffle(ref Deck deck, Random random)
        {
            for (int i = 0; i < deck.Cards.Length - 1; i++)
            {
                int r = random.Next(i, deck.Cards.Length);
                Cards a = deck.Cards[i];
                deck.Cards[i] = deck.Cards[r];
                deck.Cards[r] = a;
            }
        }

        static void InitPositionCard(ref Deck deck)
        {        
            int space=10;
            int c=0;
           
            deck.Cards[0].position.X=100;
            deck.Cards[0].position.Y=100;
            
            for(int i=1;i<deck.Cards.Length;i++)    
            {
                
                    deck.Cards[i].position.X=deck.Cards[i-1].position.X+(WIDTH_CARD+space);
                    deck.Cards[i].position.Y=deck.position.Y+((HEIGHT_CARD+space)*c);
                    
                
                if(i!=0&&i%deck.NumCol==0)
                {
                    c++;
                    deck.Cards[i].position.X=deck.position.X;
                    deck.Cards[i].position.Y=deck.position.Y+((HEIGHT_CARD+space)*c);
                }
            }
        }

        static void PrintDeck(Window window, Deck deck, int numCards)
        {   
            for (int i = 0; i < numCards; i++)
            {
                    if(deck.Cards[i].Show==true)
                    {
                        DrawFullRect(window, deck.Cards[i].position.X, deck.Cards[i].position.Y, WIDTH_CARD, HEIGHT_CARD, deck.Cards[i].color.r, deck.Cards[i].color.g, deck.Cards[i].color.b);    
                    }
                    else
                    {
                        DrawFullRect(window, deck.Cards[i].position.X, deck.Cards[i].position.Y, WIDTH_CARD, HEIGHT_CARD, 195,195,195);
                    }

            }
        }

        static int SelectCard(Window window,ref Deck deck)
        {
            for(int i=0;i<deck.Cards.Length;i++)
           {
                if(window.mouseLeft&&(window.mouseX >= deck.Cards[i].position.X && window.mouseX < (deck.Cards[i].position.X+WIDTH_CARD))&&(window.mouseY >= deck.Cards[i].position.Y && window.mouseY < (deck.Cards[i].position.Y+HEIGHT_CARD)))
                {
                    return i;
                }             
            }
           return -1;
        }


        static void ClearWindow(Window window)
        {
            for (int i = 0; i < window.bitmap.Length; i++)
            {
                window.bitmap[i] = 0;
            }
        }
        
        static bool Win(ref Deck deck)
        {
            int c = 0;

            for (int i = 0; i < deck.Cards.Length; i++)
            {
                if (deck.Cards[i].Show == true) c++;
            }

            if (c == deck.Cards.Length - 2) return true;
            else return false;
        }

        static void Main(string[] args)
        {
            float clickcountdown=0;

           

            Window window = new Window(800, 800, "Memory", PixelFormat.RGB);

            Random random = new Random();

            Deck deck;

            int[] selectedCards=new int[2];
            
            int nrSelectedCards=0;

            InitDeck(out deck, NUM_CARD, NUM_COL);

            int pairToFind=deck.Cards.Length/2;

            Shuffle(ref deck, random);

            InitPositionCard(ref deck);

            while(pairToFind > 0)
            {
                ClearWindow(window);
                //input 
                if(window.GetKey(KeyCode.Esc))
                {
                    return;
                }
                //game state
                clickcountdown -= window.deltaTime;
                
                
                if(nrSelectedCards < 2)
                {
                    if(clickcountdown <= 0)
                    {
                        int nextCard=SelectCard(window, ref deck);
                        if(nextCard != -1)
                        {
                            clickcountdown=DEFAULT_CLICK_COUNTDOWN;
                            if(!deck.Cards[nextCard].Show)
                            {
                                deck.Cards[nextCard].Show=true;
                                selectedCards[nrSelectedCards++] = nextCard;
                            }
                        }
                    }
                }
                               
                PrintDeck(window,deck,NUM_CARD);
                
                window.Blit();

                if(nrSelectedCards == 2)
                {
                    if(deck.Cards[selectedCards[0]].Number == deck.Cards[selectedCards[1]].Number)
                    {
                        pairToFind--;
                    }                
                    else        
                    {
                        deck.Cards[selectedCards[0]].Show = false;
                        deck.Cards[selectedCards[1]].Show = false;
                        
                        Thread.Sleep(1000);                    
                    } 
                    nrSelectedCards=0;
                }
 
            }
        }
    }
}
