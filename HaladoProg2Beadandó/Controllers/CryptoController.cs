﻿using HaladoProg2Beadandó.Data;
using HaladoProg2Beadandó.Models.DTOs;
using HaladoProg2Beadandó.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HaladoProg2Beadandó.Controllers
{
    [Route("api/trade")]
    [ApiController]
    public class CryptoController : DataContextController
    {
        private readonly IMapper mapper;
        public CryptoController(DataContext context, IMapper mapper) : base(context) {
            this.mapper = mapper;
        }


        [HttpPost("buy")]
        public async Task<IActionResult> BuyCrypto(int userId, [FromBody] BuyCryptoDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.VirtualWallet)
                .ThenInclude(w => w.CryptoAssets)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound("Felhasználó nem található");

            var crypto = await _context.CryptoCurrencies
                .FirstOrDefaultAsync(c => c.Symbol == dto.Symbol);

            if (crypto == null)
                return NotFound("Ilyen kriptovaluta nem létezik");

            double totalCost = dto.AmountToBuy * crypto.Price;

            if (user.VirtualWallet.Amount < totalCost)
                return BadRequest("Nincs elég pénz az egyenlegen");

            user.VirtualWallet.Amount -= totalCost;
            if (crypto.Amount < dto.AmountToBuy)
                return BadRequest("Nincs elég elérhető mennyiség ebből a kriptovalutából");
            crypto.Amount -= dto.AmountToBuy;
            
            var existingAsset = user.VirtualWallet.CryptoAssets
                .FirstOrDefault(c => c.Symbol == dto.Symbol);

            if (existingAsset != null)
            {
                existingAsset.Amount += dto.AmountToBuy;
                existingAsset.Price += totalCost;
            }
            else
            {
                var newAsset = new CryptoAsset
                {
                    Symbol = dto.Symbol,
                    Amount = dto.AmountToBuy,
                    Price = totalCost,
                    CryptoCurrencyName = dto.CryptoCurrencyName,
                    VirtualWalletId = user.VirtualWallet.VirtualWalletId,
                };
                _context.CryptoAssets.Add(newAsset);
            }

            await _context.SaveChangesAsync();
            return Ok("Sikeres vásárlás");

        }

        [HttpPost("sell")]
        public async Task<IActionResult> SellCrypto(int userId,[FromBody] SellCryptoDTO dto)
            {
                var user = await _context.Users
               .Include(u => u.VirtualWallet)
               .ThenInclude(w => w.CryptoAssets)
               .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return NotFound("Felhasználó nem található");

                var crypto = await _context.CryptoCurrencies
                    .FirstOrDefaultAsync(c => c.Symbol == dto.Symbol);

                var cryptoAsset = user.VirtualWallet.CryptoAssets
    .           FirstOrDefault(ca => ca.Symbol == dto.Symbol);

            if (crypto == null)
                    return NotFound("Ilyen kriptovaluta nem létezik");
             
                 if (crypto.Amount < dto.AmountToSell)
                    return BadRequest("Nem tudsz ennyit eladni");
                 cryptoAsset.Amount -= dto.AmountToSell;
                 double totalSale = dto.AmountToSell * crypto.Price;
                 cryptoAsset.Price -= totalSale;
                 crypto.Amount += dto.AmountToSell;
                 if (cryptoAsset.Amount == 0)
                _context.CryptoAssets.Remove(cryptoAsset);

                // Növeld a felhasználó pénztárcájának egyenlegét
                 user.VirtualWallet.Amount += totalSale;





            await _context.SaveChangesAsync();
            return Ok("Sikeres eladás");
            }


        //TODO PORTFOLIO
    }
}
