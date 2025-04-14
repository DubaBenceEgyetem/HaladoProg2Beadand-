﻿using HaladoProg2Beadandó.Data;
using HaladoProg2Beadandó.Models.DTOs;
using HaladoProg2Beadandó.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2Beadandó.Controllers
{
    [Route("api/trade")]
    [ApiController]
    public class BuyCryptoController : DataContextController
    {

        public BuyCryptoController(DataContext context) : base(context) { }


        [HttpPost("buy")]
        public async Task<IActionResult> BuyCrypto(int userId, [FromBody] BuyCryptoDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.VirtualWallet)
                .ThenInclude(w => w.CryptoAssets)
                .FirstOrDefaultAsync(u => u.UserId == u.VirtualWallet.UserId);

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
                Console.WriteLine($"Found asset: {existingAsset.Symbol} with amount: {existingAsset.Amount}");
                existingAsset.Amount += dto.AmountToBuy;
                existingAsset.Price += totalCost;
                Console.WriteLine(totalCost);
                Console.WriteLine($"Updated amount: {existingAsset.Amount}");
            }
            else
            {
                var newAsset = new CryptoAsset
                {
                    Symbol = dto.Symbol,
                    Amount = dto.AmountToBuy,
                    CryptoCurrencyName = dto.CryptoCurrencyName,
                    VirtualWalletId = user.VirtualWallet.VirtualWalletId,
                };
                _context.CryptoAssets.Add(newAsset);
            }

            await _context.SaveChangesAsync();

            return Ok("Sikeres vásárlás");
        }



    }
}
